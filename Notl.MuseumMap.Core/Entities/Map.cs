using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.Core.Entities
{
    public class Map : EntityBase
    {
        public string? Name { get; set; }
        public ImageReference? Image { get; set; }
    }
}
