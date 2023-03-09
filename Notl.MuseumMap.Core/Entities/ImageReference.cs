using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.Core.Entities
{
    public class ImageReference
    {
        public Guid Id { get; set; }
        public string? Url { get; set; }
        public string? Thumbnail { get; set; }
    }
}
