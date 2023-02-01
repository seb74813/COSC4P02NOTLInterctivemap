using Microsoft.Extensions.Configuration;

namespace Notl.MuseumMap.Web
{
    public class EnvironmentDetector
    {
        /// <summary>
        /// Detects which environment we should use using the current browser's address.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="baseAddress"></param>
        /// <returns></returns>
        public static string Detect(IConfiguration configuration, string baseAddress)
        {
            string? environment = null;
            var detection = configuration.GetSection("EnvironmentDetection");
            if (detection != null)
            {
                var settings = detection.GetChildren();
                environment = settings?.FirstOrDefault(s => baseAddress.Contains(s["Host"] ?? "<invalid>"))?["Environment"];
            }

            if(string.IsNullOrWhiteSpace(environment))
            {
                environment = "Production;";
            }
            return environment;
        }
    }
}
