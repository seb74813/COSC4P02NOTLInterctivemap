using Microsoft.AspNetCore.Mvc;
using Notl.MuseumMap.Api.Models;
using Notl.MuseumMap.Api.Models.Common;
using Notl.MuseumMap.Core.Common;
using Notl.MuseumMap.Core.Entities;
using Notl.MuseumMap.Core.Managers;
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
        readonly MapManager mapManager;

        /// <summary>
        /// Constructs the controller with dependencies.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        /// <param name="mapManager"></param>
        public MapController(ILogger<MapController> logger, MuseumMapOptions options, MapManager mapManager)
            :base(logger, options)
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

        /// <summary>
        /// Create point of interest.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Route("poi")]
        [HttpPost]
        [ProducesResponseType(typeof(PointOfInterest), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> CreatePOIAsync([FromQuery] string? data)
        {
            try
            {
                var poi = await mapManager.CreatePOIAsync(data);
                return Ok(poi);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}