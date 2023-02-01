using Microsoft.Extensions.Configuration;

namespace Notl.MuseumMap.App.Common
{
    /// <summary>
    /// Manages the Client Context Options.
    /// </summary>
    public class ClientContextOptions
    {
        /// <summary>
        /// Constructs the Client Context Options.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        /// <exception cref="ApplicationException"></exception>
        public ClientContextOptions(IConfiguration configuration, string environment)
        {
            // Get the backend service configuration.
            MuseumMapServiceBaseUrl = configuration[$"{environment}:MuseumMapService:BaseUrl"] ?? throw new ApplicationException("Missing appsettings.json entry for {environment}:MuseumMapService:BaseUrl");
        }

        /// <summary>
        /// Constructor with url information
        /// </summary>
        /// <param name="museumMapServiceBaseUrl"></param>
        public ClientContextOptions(string museumMapServiceBaseUrl)
        {
            MuseumMapServiceBaseUrl = museumMapServiceBaseUrl;
        }

        /// <summary>
        /// The base URL for the backend API.
        /// </summary>
        public string MuseumMapServiceBaseUrl { get; set; }
    }
}
