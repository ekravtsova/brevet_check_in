using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using brevet_tracker.Server.Constants;
using brevet_tracker.Server.Models.Auth;
using brevet_tracker.Server.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace brevet_tracker.Server.Controllers
{
    /// <summary>
    /// Provides admin endpoints for assigning, removing, and listing user roles.
    /// </summary>
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    [Route("api/admin/roles")]
    public class RoleController : AuthorizedApiControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(IRoleService roleService, UserManager<ApplicationUser> userManager)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        /// <param name="request">The role assignment payload.</param>
        /// <returns>An operation result indicating success or failure.</returns>
        [HttpPost("assign")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignRole([FromBody] RoleAssignmentRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var availableRoles = GetAvailableRoleNames();
            if (!availableRoles.Contains(request.RoleName, StringComparer.OrdinalIgnoreCase))
            {
                return NotFound("Role not found.");
            }

            var success = await _roleService.AssignRoleToUserAsync(request.UserId, request.RoleName);
            if (!success)
            {
                return BadRequest("Failed to assign role. Ensure the role exists and request is valid.");
            }

            return Ok(new { message = "Role assigned successfully." });
        }

        /// <summary>
        /// Removes a role from a user.
        /// </summary>
        /// <param name="request">The role removal payload.</param>
        /// <returns>An operation result indicating success or failure.</returns>
        [HttpPost("remove")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveRole([FromBody] RoleAssignmentRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var availableRoles = GetAvailableRoleNames();
            if (!availableRoles.Contains(request.RoleName, StringComparer.OrdinalIgnoreCase))
            {
                return NotFound("Role not found.");
            }

            var success = await _roleService.RemoveRoleFromUserAsync(request.UserId, request.RoleName);
            if (!success)
            {
                return BadRequest("Failed to remove role. Ensure the role exists and request is valid.");
            }

            return Ok(new { message = "Role removed successfully." });
        }

        /// <summary>
        /// Gets all roles assigned to the specified user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A list of role names assigned to the user.</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IList<string>>> GetUserRoles([FromRoute] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User id is required.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var roles = await _roleService.GetUserRolesAsync(userId);
            return Ok(roles);
        }

        /// <summary>
        /// Gets all available application roles.
        /// </summary>
        /// <returns>A list of available role names.</returns>
        [HttpGet("list")]
        [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<IList<string>> ListRoles()
        {
            return Ok(GetAvailableRoleNames());
        }

        private static List<string> GetAvailableRoleNames()
        {
            // Reflect over constants so newly added role constants appear automatically.
            return typeof(RoleNames)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(field => field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                .Select(field => field.GetRawConstantValue()?.ToString())
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(role => role)
                .ToList();
        }
    }

    /// <summary>
    /// Represents a request to assign or remove a role for a user.
    /// </summary>
    public class RoleAssignmentRequestDto
    {
        /// <summary>
        /// Gets or sets the target user identifier.
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the role name to assign or remove.
        /// </summary>
        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}
