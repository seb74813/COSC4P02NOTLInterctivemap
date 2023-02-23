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

        public AdminManager(DbManager dbManager)
        {
            this.dbManager = dbManager;
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
            return await storageManager.UploadFileAndCreateThumbnail(StorageContainerType.Messages, mapId, photoFilename, photoStream);
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
        public async Task<PointOfInterest> CreatePOIAsync(Guid id, Guid mapId, double x, double y, POIType pOIType)
        {
            // Create POI
            var poi = new PointOfInterest { Id = id, MapId = mapId, x = x, y = y, POIType = pOIType };

            // Add to the database and return
            await dbManager.CreateAsync(poi);
            return poi;
        }

        public async Task<PointOfInterest> UpdatePOIAsync(Guid id, Guid mapId, double x, double y, POIType pOIType)
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

        public async Task<Map> CreateMapAsync(Guid id, string image)
        {
            // Create map
            var map = new Map { Id = id, ImageUrl = image };

            // Add to the database and return
            await dbManager.CreateAsync(map);
            return map;
        }

        public async Task DeletePOIAsync(Guid id)
        {
            var poi = await dbManager.GetAsync<PointOfInterest>(id, Partition.Calculate(id));

            if (poi == null)
            {
                throw new MuseumMapException(MuseumMapErrorCode.InvalidPOIError);
            }

            await dbManager.DeleteAsync(poi);

        }

        public async Task DeleteMapAsync(Guid id)
        {
            var map = await dbManager.GetAsync<Map>(id, Partition.Calculate(id));

            if (map == null)
            {
                throw new MuseumMapException(MuseumMapErrorCode.InvalidMapError);
            }

            await dbManager.DeleteAsync(map);

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
