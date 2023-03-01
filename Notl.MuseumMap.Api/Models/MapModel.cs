using Notl.MuseumMap.Core.Entities;

namespace Notl.MuseumMap.Api.Models
{
    /// <summary>
    /// A model for the map
    /// </summary>
    public class MapModel
    {
        /// <summary>
        /// Converts a map into a model
        /// </summary>
        /// <param name="map"></param>
        public MapModel(Map map)
        {
            Id = map.Id;
            Image = map.Image;
        }

        /// <summary>
        /// The id of the map
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// The url to the image of the map
        /// </summary>
        public ImageReference? Image { get; set; }
    }
}
