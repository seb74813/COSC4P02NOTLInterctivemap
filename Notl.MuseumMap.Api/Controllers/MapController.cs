using Microsoft.AspNetCore.Mvc;
using Notl.MuseumMap.Api.Models;
using Notl.MuseumMap.Api.Models.Common;
using Notl.MuseumMap.Api.Models.Map;
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
        /// Creates a point of interest.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("poi")]
        [HttpPost]
        [ProducesResponseType(typeof(POIModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> CreatePOIAsync([FromQuery] POIModel model)
        {
            try
            {
                // POI Validation
                if (model.x < 0 || model.x >= 100 || model.x < 0 || model.x >= 100)
                {
                    throw new MuseumMapException(MuseumMapErrorCode.InvalidPOIError, "Cordinates are out of bounds");
                }

                // Add POI to the database
                var poi = await mapManager.CreatePOIAsync(Guid.NewGuid(), model.MapId, model.x, model.y, model.POIType);
                return Ok(new POIModel(poi));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Get a point of interest.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("poi")]
        [HttpGet]
        [ProducesResponseType(typeof(POIModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> GetPOIAsync([FromQuery] Guid id)
        {
            try
            {
                // Get POI from the database
                var poi = await mapManager.GetPOIAsync(id);
                return Ok(new POIModel(poi));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}