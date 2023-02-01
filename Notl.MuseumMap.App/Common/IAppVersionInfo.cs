using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.App.Common
{
    /// <summary>
    /// Application version information.
    /// </summary>
    public interface IAppVersionInfo
    {
        /// <summary>
        /// Version number.
        /// </summary>
        string? VersionNumber { get; }

        /// <summary>
        /// Build number.
        /// </summary>
        string? BuildNumber { get; }
    }
}
