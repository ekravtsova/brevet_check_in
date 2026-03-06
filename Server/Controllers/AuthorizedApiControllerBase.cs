using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace brevet_tracker.Server.Controllers
{
    /// <summary>
    /// Base API controller that requires authenticated access by default.
    /// </summary>
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public abstract class AuthorizedApiControllerBase : ControllerBase
    {
    }
}
