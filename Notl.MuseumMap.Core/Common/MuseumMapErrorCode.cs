using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.Core.Common
{
    /// <summary>
    /// Error codes for the application.
    /// </summary>
    public enum MuseumMapErrorCode
    {
        /// <summary>
        /// There is an error in the configuration of the app.
        /// </summary>
        ConfigurationError,

        InvalidPOIError,

        ActiveMapError
    }
}
