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
        /// Example protected brevet endpoint.
        /// </summary>
        [HttpGet("sample")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Sample()
        {
            return Ok(new { message = "Brevet endpoint is protected." });
        }
    }
}
