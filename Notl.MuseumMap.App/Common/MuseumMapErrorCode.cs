using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.App.Common
{
    /// <summary>
    /// Error codes for the application.
    /// </summary>
    public enum MuseumMapErrorCode
    {
        /// <summary>
        /// A channel related error has occurred.
        /// </summary>
        ChannelTokenNotFound,

        /// <summary>
        /// There is an error in the configuration of the app.
        /// </summary>
        ConfigurationError,

        /// <summary>
        /// The channel information is invalid.
        /// </summary>
        ChannelInfoInvalid,

        /// <summary>
        /// The device automatic sign-up failed.
        /// </summary>
        DeviceSignUpFailed,

        /// <summary>
        /// Indicates the Azure Event Hub was not initialized correctly.
        /// </summary>
        EventHubNotIntialized
    }
}
