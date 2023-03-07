using Azure.Storage.Blobs.Models;
using Notl.MuseumMap.Core.Common;
using Notl.MuseumMap.Core.Entities;
using Notl.MuseumMap.Core.Tools;

namespace Notl.MuseumMap.Core.Managers
{
    public class AdminManager
    {
        private readonly Guid configId = new Guid("00000000-0000-0000-0000-000000000001");
        readonly DbManager dbManager;
        readonly StorageManager storageManager;

        public AdminManager(DbManager dbManager, StorageManager storageManager)
        {
            this.dbManager = dbManager;
            this.storageManager = storageManager;
        }

        /// <summary>
        /// Uploads a new photo.
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="photoFilename"></param>
        /// <param name="photoStream"></param>
        /// <returns></returns>
        public async Task<ImageReference> UploadImageAsync(Guid mapId, string photoFilename, Stream photoStream)
        {
            // Upload the file into storage
            return await storageManager.UploadFileAndCreateThumbnail(StorageContainerType.PublicMaps, mapId, photoFilename, photoStream);
        }

        /// <summary>
        /// Creates a map in the database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<Map> CreateMapAsync(Guid id)
        {
            // Create map
            var map = new Map { Id = id};

            // Add to the database and return
            await dbManager.CreateAsync(map);
            return map;
        }

        /// <summary>
        /// Gets the active map
        /// </summary>
        /// <returns></returns>
        public async Task<Map> GetActiveMapAsync()
        {
            return await GetActiveMapInternalAsync();
        }

        /// <summary>
        /// Gets a Map from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Map> GetMapAsync(Guid id)
        {
            var map = await dbManager.GetAsync<Map>(id, Partition.Calculate(id));

            if (map == null)
            {
                throw new MuseumMapException(MuseumMapErrorCode.InvalidMapError);
            }

            return map;
        }

        /// <summary>
        /// Gets all the maps in the database
        /// </summary>
        /// <returns></returns>
        public async Task<List<Map>> GetMapsAsync()
        {
            // Get account's posts
            var sqlQuery = $"select * from {DbManager.GetContainerName<Map>()} a where a.deleted = null";
            var maps = await dbManager.QueryAsync<Map>(sqlQuery);

            return maps;
        }

        /// <summary>
        /// Updates a map in the database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public async Task<Map> UpdateMapAsync(Guid id, ImageReference image)
        {
            var map = await dbManager.GetAsync<Map>(id, Partition.Calculate(id));
            if (map == null)
            {
                throw new MuseumMapException(MuseumMapErrorCode.InvalidMapError);
            }

            map.Image = image;

            // Update the map on the database
            await dbManager.UpdateAsync(map);
            return map;
        }

        /// <summary>
        /// Deletes a map
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="MuseumMapException"></exception>
        public async Task DeleteMapAsync(Guid id)
        {
            var map = await dbManager.GetAsync<Map>(id, Partition.Calculate(id));
            if (map == null)
            {
                throw new MuseumMapException(MuseumMapErrorCode.InvalidMapError);
            }

            await dbManager.DeleteAsync(map);
        }

        /// <summary>
        /// Creates a POI associated with a map in the database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mapId"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pOIType"></param>
        /// <returns></returns>
        public async Task<PointOfInterest> CreatePOIAsync(Guid id, Guid mapId, int x, int y, POIType pOIType)
        {
            // Create POI
            var poi = new PointOfInterest { Id = id, MapId = mapId, x = x, y = y, POIType = pOIType };

            // Add to the database and return
            await dbManager.CreateAsync(poi);
            return poi;
        }

        /// <summary>
        /// Gets a POI from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PointOfInterest> GetPOIAsync(Guid id)
        {
            var poi = await dbManager.GetAsync<PointOfInterest>(id, Partition.Calculate(id));

            var map = await GetActiveMapInternalAsync();
            if (poi == null || poi.MapId != map.Id)
            {
                throw new MuseumMapException(MuseumMapErrorCode.InvalidPOIError);
            }

            return poi;
        }

        /// <summary>
        /// Gets all the POI for a map in the database
        /// </summary>
        /// <returns></returns>
        public async Task<List<PointOfInterest>> GetPOIsAsync(Guid mapId)
        {
            // Get account's posts
            var sqlQuery = $"select * from {DbManager.GetContainerName<PointOfInterest>()} a where a.deleted = null and a.MapId = @id";
            var parameters = new Dictionary<string, object>
            {
                {"@id", mapId},
            };
            var pois = await dbManager.QueryAsync<PointOfInterest>(sqlQuery, parameters);

            return pois;
        }

        /// <summary>
        /// Updates a POI
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mapId"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="pOIType"></param>
        /// <returns></returns>
        /// <exception cref="MuseumMapException"></exception>
        public async Task<PointOfInterest> UpdatePOIAsync(Guid id, Guid mapId, int x, int y, POIType pOIType)
        {
            // Get POI
            var poi = await dbManager.GetAsync<PointOfInterest>(id, Partition.Calculate(id));
            if (poi == null)
            {
                throw new MuseumMapException(MuseumMapErrorCode.InvalidPOIError);
            }

            // Update position and type
            poi.x = x; poi.y = y;
            poi.POIType = pOIType;

            // Add to the database and return
            await dbManager.UpdateAsync(poi);
            return poi;
        }

        /// <summary>
        /// Updates the content of a POI
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newTitle"></param>
        /// <param name="newDesc"></param>
        /// <param name="newImageURL"></param>
        /// <returns></returns>
        /// <exception cref="MuseumMapException"></exception>
        public async Task<PointOfInterest> UpdatePOIContentAsync(Guid id, string? newTitle, string? newDesc, string? newImageURL)
        {
            // Get POI
            var poi = await dbManager.GetAsync<PointOfInterest>(id, Partition.Calculate(id));
            if (poi == null)
            {
                throw new MuseumMapException(MuseumMapErrorCode.InvalidPOIError);
            }

            // Update POI content
            poi.Title = newTitle;
            poi.Description= newDesc;
            poi.ImageURL = newImageURL;

            // Add to the database and return
            await dbManager.UpdateAsync(poi); 
            return poi;
        }

        /// <summary>
        /// Deletes a POI
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="MuseumMapException"></exception>
        public async Task DeletePOIAsync(Guid id)
        {
            var poi = await dbManager.GetAsync<PointOfInterest>(id, Partition.Calculate(id));

            if (poi == null)
            {
                throw new MuseumMapException(MuseumMapErrorCode.InvalidPOIError);
            }

            await dbManager.DeleteAsync(poi);
        }

        private async Task<Map> GetActiveMapInternalAsync()
        {
            var config = await dbManager.GetAsync<Config>(configId, Partition.Calculate(configId));
            if (config == null || config.ActiveMap == Guid.Empty)
            {
                throw new MuseumMapException(MuseumMapErrorCode.ActiveMapError);
            }

            var map = await dbManager.GetAsync<Map>(config.ActiveMap, Partition.Calculate(config.ActiveMap));
            if (map == null)
            {
                throw new MuseumMapException(MuseumMapErrorCode.ActiveMapError);
            }

            return map;
        }
    }
}
