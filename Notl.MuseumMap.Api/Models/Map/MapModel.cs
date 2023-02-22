namespace Notl.MuseumMap.Api.Models.Map
{
    public class MapModel
    {
        public Guid Id { get; set; }
        public MapModel(Map map) { 
            Id = map.Id;
            ImageUrl= map.ImageUrl; 
        }  
        public string? ImageUrl { get; set; }
    }
}
