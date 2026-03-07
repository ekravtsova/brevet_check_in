using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace brevet_tracker.Server.Controllers
{
    /// <summary>
    /// Protected brevet endpoints.
    /// </summary>
    [Route("api/[controller]")]
    public class BrevetController : AuthorizedApiControllerBase
    {
        /// <summary>
        /// Gets all brevets.
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "ParticipantOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetAll()
        {
            return Ok(new { message = "Get all brevets (participant or admin)." });
        }

        /// <summary>
        /// Gets a brevet by identifier.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "ParticipantOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            return Ok(new { message = $"Get brevet {id} (participant or admin)." });
        }

        /// <summary>
        /// Creates a brevet.
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult Create()
        {
            return Ok(new { message = "Create brevet (admin only)." });
        }

        /// <summary>
        /// Updates a brevet.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(int id)
        {
            return Ok(new { message = $"Update brevet {id} (admin only)." });
        }

        /// <summary>
        /// Deletes a brevet.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            return Ok(new { message = $"Delete brevet {id} (admin only)." });
        }
    }
}
