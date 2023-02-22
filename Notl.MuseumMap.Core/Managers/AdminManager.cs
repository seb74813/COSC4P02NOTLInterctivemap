using Microsoft.AspNetCore.Components;
using Microsoft.Azure.Cosmos;
using Notl.MuseumMap.Admin.Common;
using Notl.MuseumMap.Core.Common;
using Notl.MuseumMap.Core.Entities;
using Notl.MuseumMap.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using MuseumMapErrorCode = Notl.MuseumMap.Core.Common.MuseumMapErrorCode;
using MuseumMapException = Notl.MuseumMap.Core.Common.MuseumMapException;

namespace Notl.MuseumMap.Core.Managers
{
    public class AdminManager
    {
        private readonly Guid configId = new Guid("00000000-0000-0000-0000-000000000001");
        readonly DbManager dbManager;

        public AdminManager(DbManager dbManager)
        {
            this.dbManager = dbManager;
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
