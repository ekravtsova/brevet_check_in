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
        /// Example protected checkpoint endpoint.
        /// </summary>
        [HttpGet("sample")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Sample()
        {
            return Ok(new { message = "Checkpoint endpoint is protected." });
        }
    }
}
