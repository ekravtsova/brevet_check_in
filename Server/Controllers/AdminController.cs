using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace brevet_tracker.Server.Controllers
{
    /// <summary>
    /// Endpoints restricted to admin users.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class AdminController : AuthorizedApiControllerBase
    {
        /// <summary>
        /// Example admin-only endpoint.
        /// </summary>
        [HttpGet("sample")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult Sample()
        {
            return Ok(new { message = "Admin endpoint is role-protected." });
        }
    }
}
