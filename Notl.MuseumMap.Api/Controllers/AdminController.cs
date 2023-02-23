using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Adds a new photo to storage.
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [Route("map/photo")]
        [ProducesResponseType(typeof(ImageReference), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        [HttpPost]
        public async Task<IActionResult> UploadMapImageAsync(Guid mapId, IFormFile file)
        {
            try
            {
                var image = await adminManager.UploadImageAsync(mapId, file.FileName, file.OpenReadStream());

                return Ok(image);
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
                // POI Validation
                if (model.x < 0 || model.x >= 100 || model.x < 0 || model.x >= 100)
                {
                    throw new MuseumMapException(MuseumMapErrorCode.InvalidPOIError, "Cordinates are out of bounds");
                }

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
                // POI Validation
                if (model.x < 0 || model.x >= 100 || model.x < 0 || model.x >= 100)
                {
                    throw new MuseumMapException(MuseumMapErrorCode.InvalidPOIError, "Cordinates are out of bounds");
                }

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
        /// Deletes a point of interest.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("poi")]
        [HttpDelete]
        [ProducesResponseType(typeof(POIModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> DeletePOIAsync(Guid id)
        {
            try
            {
                await adminManager.DeletePOIAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        /// <summary>
        /// Deletes a map 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("map")]
        [HttpDelete]
        [ProducesResponseType(typeof(POIModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> DeleteMapAsync(Guid id)
        {
            try
            {
                await adminManager.DeleteMapAsync(id);
                return Ok();
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
                // POI Validation
                if (model.x < 0 || model.x >= 100 || model.x < 0 || model.x >= 100)
                {
                    throw new MuseumMapException(MuseumMapErrorCode.InvalidPOIError, "Cordinates are out of bounds");
                }
                

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
        /// Creates a map.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        [Route("map")]
        [HttpPost]
        [ProducesResponseType(typeof(MapModel), 200)]
        [ProducesResponseType(typeof(ErrorModel), 400)]
        public async Task<IActionResult> CreateMapAsync([FromQuery] string image)
        {
            try
            {
                // Add map to the database
                var map = await adminManager.CreateMapAsync(Guid.NewGuid(), image);
                return Ok(new MapModel(map));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}