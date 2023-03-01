using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace Notl.MuseumMap.Core.Common
{
    public class MuseumMapOptions
    {
        public MuseumMapOptions(IConfiguration configuration)
        {
            ConnectionString = configuration["Storage:ConnectionString"] ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string ConnectionString { get; set; }
    }
}