using Microsoft.Azure.Cosmos;
using Notl.MuseumMap.Core.Common;
using Notl.MuseumMap.Core.Entities;
using Notl.MuseumMap.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.Core.Managers
{
    public class MapManager
    {
        readonly DbManager dbManager;

        public MapManager(DbManager dbManager) 
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

        /// <summary>
        /// Gets a POI from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PointOfInterest> GetPOIAsync(Guid id)
        {
            var poi = await dbManager.GetAsync<PointOfInterest>(id, Partition.Calculate(id));
            if (poi == null)
            {
                throw new MuseumMapException(MuseumMapErrorCode.InvalidPOIError);
            }
            return poi;
        }
    }
}
