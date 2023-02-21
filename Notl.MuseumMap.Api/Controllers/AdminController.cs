using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notl.MuseumMap.Api.Models;
using Notl.MuseumMap.Api.Models.Common;
using Notl.MuseumMap.Core.Common;
using Notl.MuseumMap.Core.Managers;

namespace Notl.MuseumMap.Api.Controllers
{
    /// <summary>
    /// Administrator functions in the application.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AdminController : BaseController
    {
        readonly MapManager mapManager;

        /// <summary>
        /// Constructs the controller with dependencies.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        /// <param name="mapManager"></param>
        public AdminController(ILogger<MapController> logger, MuseumMapOptions options, MapManager mapManager)
            : base(logger, options)
        {
            this.mapManager = mapManager;
        }

        /// <summary>
        /// Sample ping method.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("ping")]
        [HttpGet]
        [Authorize(Roles = "Administrator,Readers")]
        [ProducesResponseType(typeof(SampleData), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> PingAsync([FromQuery] string? data)
        {
            try
            {
                await Task.CompletedTask;

                foreach(var claim in Request.HttpContext.User.Claims)
                {
                    data += $"<br />{claim.Type}={claim.Value}";
                }

                return Ok(new SampleData { Data = data?.ToUpper() });
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

    }
}
