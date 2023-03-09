using Notl.MuseumMap.Core.Entities;
using Notl.MuseumMap.Core.Tools;
using System.Drawing.Drawing2D;

namespace Notl.MuseumMap.Api.Models
{
    /// <summary>
    /// A model for Points of Interest
    /// </summary>
    public class POIModel
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public POIModel()
        {

        }

        /// <summary>
        /// Creates an ready existing POI
        /// </summary>
        /// <param name="poi"></param>
        public POIModel(PointOfInterest poi)
        {
            Id = poi.Id;
            MapId = poi.MapId;
            x = poi.x;
            y = poi.y;
            POIType = poi.POIType;
            Description = poi.Description;
            Title = poi.Title;
            ImageURL = poi.ImageURL;
        }

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
        public int x { get; set; }

        /// <summary>
        /// The location on the y axis
        /// </summary>
        public int y { get; set; }

        /// <summary>
        /// The URL of the image
        /// </summary>
        public string? ImageURL { get; set; }

        /// <summary>
        /// The Title text of the POI
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The description of the POI
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The type of Point of Interest
        /// </summary>
        public POIType POIType { get; set; }

    }
}
