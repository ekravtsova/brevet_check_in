using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace brevet_tracker.Server.Controllers
{
    /// <summary>
    /// Endpoints for users marking and viewing checkpoint progress.
    /// </summary>
    [Route("api/[controller]")]
    public class UserMarkController : AuthorizedApiControllerBase
    {
        /// <summary>
        /// Creates a user mark entry.
        /// </summary>
        [HttpPost("mark")]
        [Authorize(Policy = "ParticipantOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult PostMark()
        {
            return Ok(new { message = "Post mark (participant or admin)." });
        }

        /// <summary>
        /// Gets marks for the current request scope.
        /// </summary>
        [HttpGet("marks")]
        [Authorize(Policy = "ParticipantOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetMarks()
        {
            return Ok(new { message = "Get marks (participant or admin)." });
        }
    }
}
