using Majorsoft.Blazor.Components.Common.JsInterop.Geo;
using Microsoft.JSInterop;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace Notl.MuseumMap.Admin.Common
{ 
    /// <summary>
    /// Web host device information provider.
    /// </summary>
    public class BrowserAppDeviceInfo : IAppDeviceInfo
    {
        readonly IGeolocationService geolocationService;

        /// <summary>
        /// Constructs  the app device information with dependencies.
        /// </summary>
        /// <param name="geolocationService"></param>
        public BrowserAppDeviceInfo(IGeolocationService geolocationService)
        {
            this.geolocationService = geolocationService;
        }

        /// <summary>
        /// True if the app is browser hosted.
        /// </summary>
        public bool IsBrowserHosted 
        { 
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the platform information.
        /// </summary>
        public DevicePlatformType Platform
        {
            get
            {
                return DevicePlatformType.Web;
            }
        }

        /// <summary>
        /// Provides access to the system theme settings.
        /// </summary>
        public AppTheme SystemTheme
        {
            get
            {
                return AppTheme.Light;
            }
        }

        /// <summary>
        /// Gets the current location and calls back the handler.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task GetCurrentLocationAsync(Func<Location?, Task> callback, TimeSpan timeout)
        {
            await geolocationService.GetCurrentPositionAsync(async (result) => 
            {
                if(result == null || result.IsSuccess == false || result.Coordinates == null)
                {
                    await callback.Invoke(null);
                }
                else
                {
                    await callback.Invoke(new Location { Latitude = result.Coordinates.Latitude, Longitude = result.Coordinates.Longitude });
                }
            }, 
            false, timeout, timeout);
        }
    }
}
