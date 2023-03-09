using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Notl.MuseumMap.Core.Common;
using Notl.MuseumMap.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.Tests
{
    public class TestHelper
    {
        readonly static HttpClient http = new();
        readonly static IConfiguration configuration; 
        readonly static ILoggerFactory loggerFactory; 
        public readonly static DbManager Db; 
        public readonly static MuseumMapOptions Options;

        static TestHelper() 
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

            var serviceProvider = new ServiceCollection()
                .AddLogging((o) => { o.AddDebug(); o.AddConsole(); o.AddSimpleConsole(); })
                .BuildServiceProvider() ?? throw new ApplicationException("Service provider not found");

#pragma warning disable CS8601 // Possible null reference assignment.            
            loggerFactory = serviceProvider.GetService<ILoggerFactory>();
#pragma warning restore CS8601 // Possible null reference assignment.

            if (loggerFactory == null) 
            { 
                throw new ApplicationException("Logger factory not initialized"); 
            }

            configuration = builder.Build(); 
            Options = new MuseumMapOptions(configuration); 
            Db = new DbManager(new DbManagerOptions(configuration), loggerFactory.CreateLogger<DbManager>());
        }
    }
}
