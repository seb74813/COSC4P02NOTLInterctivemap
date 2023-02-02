using Notl.MuseumMap.Core.Entities;
using Notl.MuseumMap.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<PointOfInterest> CreatePOIAsync(string? name)
        { 
            var poi = new PointOfInterest { Name = name };
            await dbManager.CreateAsync(poi);
            return poi;
        }
    }
}
