using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> PingAsync([FromBody] string? data)
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
        /// Get a point of interest.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("poi/{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(POIModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> GetPOIAsync([FromRoute] Guid id)
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

        /// <summary>
        /// Get the active map.
        /// </summary>
        /// <returns></returns>
        [Route("map")]
        [HttpGet]
        [ProducesResponseType(typeof(MapModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> GetActiveMapAsync()
        {
            try
            {
                // Get POI from the database
                var map = await mapManager.GetActiveMapAsync();
                return Ok(new MapModel(map));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Get all the pois that belong to the active map from the database
        /// </summary>
        /// <returns></returns>
        [Route("pois")]
        [HttpGet]
        [ProducesResponseType(typeof(List<POIModel>), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> GetPOIsAsync()
        {
            try
            {
                // Get POI from the database
                var pois = await mapManager.GetPOIsAsync();

                List<POIModel> poiModels = new List<POIModel>();

                foreach (var poi in pois)
                {
                    poiModels.Add(new POIModel(poi));
                }

                return Ok(poiModels);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}