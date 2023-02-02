using Notl.MuseumMap.Core.Entities;
using Notl.MuseumMap.Core.Tools;
using System.Drawing.Drawing2D;

namespace Notl.MuseumMap.Api.Models.Map
{
    /// <summary>
    /// A model for Points of Interest
    /// </summary>
    public class POIModel
    {
        /// <summary>
        /// The Id of the POI
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The id of the map this POI belongs to
        /// </summary>
        public Guid MapId { get; set; }

        /// <summary>
        /// The location on the x axis
        /// </summary>
        public double x { get; set; }

        /// <summary>
        /// The location on the y axis
        /// </summary>
        public double y { get; set; }

        /// <summary>
        /// The type of Point of Interest
        /// </summary>
        public POIType POIType { get; set; }
    }
}
