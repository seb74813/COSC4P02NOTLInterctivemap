using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.Core.Entities
{
    public class PointOfInterest : EntityBase
    {
        public Guid MapId { get; set; }

        public double x { get; set; }

        public double y { get; set; }

        public string? ImageURL { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public POIType POIType { get; set; }

        
    }
}
