using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace brevet_tracker.Server.Controllers
{
    /// <summary>
    /// Protected checkpoint endpoints.
    /// </summary>
    [Route("api/[controller]")]
    public class CheckpointController : AuthorizedApiControllerBase
    {
        /// <summary>
        /// Gets all checkpoints.
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "ParticipantOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetAll()
        {
            return Ok(new { message = "Get all checkpoints (participant or admin)." });
        }

        /// <summary>
        /// Gets a checkpoint by identifier.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "ParticipantOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            return Ok(new { message = $"Get checkpoint {id} (participant or admin)." });
        }

        /// <summary>
        /// Creates a checkpoint.
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult Create()
        {
            return Ok(new { message = "Create checkpoint (admin only)." });
        }

        /// <summary>
        /// Updates a checkpoint.
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
            return Ok(new { message = $"Update checkpoint {id} (admin only)." });
        }

        /// <summary>
        /// Deletes a checkpoint.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            return Ok(new { message = $"Delete checkpoint {id} (admin only)." });
        }
    }
}
