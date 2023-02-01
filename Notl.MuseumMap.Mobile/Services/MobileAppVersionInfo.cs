using Notl.MuseumMap.App.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.Mobile.Services
{
    /// <summary>
    /// Mobile app version information class.
    /// </summary>
    public class MobileAppVersionInfo : IAppVersionInfo
    {
        /// <summary>
        /// Provides the version number information.
        /// </summary>
        public string VersionNumber => VersionTracking.CurrentVersion;

        /// <summary>
        /// Provies the build number.
        /// </summary>
        public string BuildNumber => VersionTracking.CurrentBuild;
    }
}
