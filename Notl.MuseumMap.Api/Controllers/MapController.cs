using Microsoft.AspNetCore.Mvc;
using Notl.MuseumMap.Api.Models;
using Notl.MuseumMap.Api.Models.Common;
using Notl.MuseumMap.Core.Common;
using System.Runtime.CompilerServices;

namespace Notl.MuseumMap.Api.Controllers
{
    /// <summary>
    /// Provides primary map controller methods.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MapController : BaseController
    {
        /// <summary>
        /// Constructs the controller with dependencies.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        public MapController(ILogger<MapController> logger, MuseumMapOptions options)
            :base(logger, options)
        {
        }

        /// <summary>
        /// Sample ping method.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("ping")]
        [HttpGet]
        [ProducesResponseType(typeof(SampleData), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> PingAsync([FromQuery]string? data)
        {
            try
            {
                await Task.CompletedTask;
                return Ok(new SampleData { Data = data?.ToUpper() });
            }
            catch(Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}