using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Mvc;
using Notl.MuseumMap.Api.Models;
using Notl.MuseumMap.Api.Models.Common;
using Notl.MuseumMap.Core.Common;
using Notl.MuseumMap.Core.Entities;
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
        readonly AdminManager adminManager;

        /// <summary>
        /// Constructs the controller with dependencies.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        /// <param name="adminManager"></param>
        public AdminController(ILogger<MapController> logger, MuseumMapOptions options, AdminManager adminManager)
            : base(logger, options)
        {
            this.adminManager = adminManager;
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
        public async Task<IActionResult> PingAuthAsync([FromQuery] string? data)
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

        /// <summary>
        /// Adds a new photo to storage.
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [Route("map/photo")]
        [ProducesResponseType(typeof(MapModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [HttpPost]
        public async Task<IActionResult> UpdateMapImageAsync(Guid mapId, IFormFile file)
        {
            try
            {
                var map = await adminManager.UpdateMapImageAsync(mapId, file.FileName, file.OpenReadStream());

                return Ok(new MapModel(map));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Creates a map.
        /// </summary>
        /// <returns></returns>
        [Route("map")]
        [HttpPost]
        [ProducesResponseType(typeof(MapModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> CreateMapAsync()
        {
            try
            {
                // Add map to the database
                var map = await adminManager.CreateMapAsync(Guid.NewGuid());
                return Ok(new MapModel(map));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Get the map.
        /// </summary>
        /// <returns></returns>
        [Route("map/active")]
        [HttpGet]
        [ProducesResponseType(typeof(MapModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> GetActiveMapAuthAsync()
        {
            try
            {
                // Get map from the database
                var map = await adminManager.GetActiveMapAsync();
                return Ok(new MapModel(map));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Get a map.
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        [Route("map/{mapId}")]
        [HttpGet]
        [ProducesResponseType(typeof(MapModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> GetMapAsync([FromRoute] Guid mapId)
        {
            try
            {
                // Get POI from the database
                var map = await adminManager.GetMapAsync(mapId);
                return Ok(new MapModel(map));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Get all maps in the database.
        /// </summary>
        /// <returns></returns>
        [Route("maps")]
        [HttpGet]
        [ProducesResponseType(typeof(List<MapModel>), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> GetMapsAsync()
        {
            try
            {
                // Get POI from the database
                var maps = await adminManager.GetMapsAsync();

                List<MapModel> mapModels = new List<MapModel>();

                foreach (var map in maps)
                { 
                    mapModels.Add(new MapModel(map));
                }

                return Ok(mapModels);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Updates the active map.
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        [Route("map/active/{mapId}")]
        [HttpPost]
        [ProducesResponseType(typeof(MapModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> UpdateActiveMapAsync([FromRoute] Guid mapId)
        {
            try
            {
                // Update the active in the database
                var map = await adminManager.SetActiveMapAsync(mapId);
                return Ok(new MapModel(map));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Deletes a map 
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        [Route("map/{mapId}")]
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> DeleteMapAsync([FromRoute] Guid mapId)
        {
            try
            {
                await adminManager.DeleteMapAsync(mapId);
                return Ok();
            }
            catch (Exception ex)
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
                // Add POI to the database
                var poi = await adminManager.CreatePOIAsync(Guid.NewGuid(), model.MapId, model.x, model.y, model.POIType);
                return Ok(new POIModel(poi));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Get a map poi.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("poi/{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(POIModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> GetPOIAuthAsync([FromRoute] Guid id)
        {
            try
            {
                // Get POI from the database
                var poi = await adminManager.GetPOIAsync(id);
                return Ok(new POIModel(poi));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Get all a map's pois in the database.
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        [Route("pois/{mapId}")]
        [HttpGet]
        [ProducesResponseType(typeof(List<POIModel>), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> GetPOIsAuthAsync([FromRoute] Guid mapId)
        {
            try
            {
                // Get POI from the database
                var pois = await adminManager.GetPOIsAsync(mapId);

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

        /// <summary>
        /// Updates a point of interest.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("poi")]
        [HttpPut]
        [ProducesResponseType(typeof(POIModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> UpdatePOIAsync([FromQuery] POIModel model)
        {
            try
            {
                // Add POI to the database
                var poi = await adminManager.UpdatePOIAsync(model.Id, model.MapId, model.x, model.y, model.POIType);
                return Ok(new POIModel(poi));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Updates the content within a point of interest
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("poi/content")]
        [HttpPut]
        [ProducesResponseType(typeof(POIModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> UpdatePOIContentAsync([FromQuery] POIModel model)
        {
            try
            {
                // Add POI to the database
                var poi = await adminManager.UpdatePOIContentAsync(model.Id, model.Title, model.Description, model.ImageURL);
                return Ok(new POIModel(poi));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Deletes a point of interest.
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        [Route("poi/{mapId}")]
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> DeletePOIAsync([FromRoute] Guid mapId)
        {
            try
            {
                await adminManager.DeletePOIAsync(mapId);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}