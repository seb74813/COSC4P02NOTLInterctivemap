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
        private readonly Guid configId = new Guid("00000000-0000-0000-0000-000000000001");
        readonly DbManager dbManager;

        public MapManager(DbManager dbManager) 
        {
            this.dbManager = dbManager;
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
        /// Gets the active map
        /// </summary>
        /// <returns></returns>
        public async Task<Map> GetActiveMapAsync()
        {
            return await GetActiveMapInternalAsync();
        }

        /// <summary>
        /// Gets all the POI in the active map from the database
        /// </summary>
        /// <returns></returns>
        public async Task<List<PointOfInterest>> GetPOIsAsync()
        {
            var map = await GetActiveMapAsync();

            // Get account's posts
            var sqlQuery = $"select * from {DbManager.GetContainerName<PointOfInterest>()} a where a.deleted = null and a.MapId = @id";
            var parameters = new Dictionary<string, object>
            {
                {"@id", map.Id},
            };
            var pois = await dbManager.QueryAsync<PointOfInterest>(sqlQuery, parameters);

            return pois;
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
