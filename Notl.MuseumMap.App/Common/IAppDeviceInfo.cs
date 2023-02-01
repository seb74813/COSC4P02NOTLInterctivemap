using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.App.Common
{
    /// <summary>
    /// Defines the platform device type.
    /// </summary>
    public enum DevicePlatformType
    {
        /// <summary>
        /// The platform is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The platform is a web browser.
        /// </summary>
        Web,

        /// <summary>
        /// The platform is an App on an android device.
        /// </summary>
        Android,

        /// <summary>
        /// The platform is an App on an iOS device.
        /// </summary>
        iOS,

        /// <summary>
        /// The platform is an App on a Windows device.
        /// </summary>
        Windows
    }

    /// <summary>
    /// Provides information about the device the application is running on.
    /// </summary>
    public interface IAppDeviceInfo
    {
        /// <summary>
        /// True if the app is running in a browser.
        /// </summary>
        bool IsBrowserHosted { get; }

        /// <summary>
        /// Provides the specific type of device being used.
        /// </summary>
        DevicePlatformType Platform { get; }

        /// <summary>
        /// Provides access to the system theme settings.
        /// </summary>
        AppTheme SystemTheme { get; }

        /// <summary>
        /// Gets the current location and then fires the callback (within a timeout)
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task GetCurrentLocationAsync(Func<Location?, Task> callback, TimeSpan timeout);
    }

    

}
