using System.Reflection;

namespace Notl.MuseumMap.Admin.Common
{
    /// <summary>
    /// App version provider for web host
    /// </summary>
    public class BrowserAppVersionInfo : IAppVersionInfo
    {
        /// <summary>
        /// The Version number.
        /// </summary>
        public string? VersionNumber => Assembly.GetExecutingAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        /// <summary>
        /// The build number.
        /// </summary>
        public string? BuildNumber => Assembly.GetExecutingAssembly()?.GetCustomAttribute<AssemblyVersionAttribute>()?.Version;
    }
}
