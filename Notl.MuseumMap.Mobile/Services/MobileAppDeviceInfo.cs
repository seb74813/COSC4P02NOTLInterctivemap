using Majorsoft.Blazor.Components.Common.JsInterop.Geo;
using Notl.MuseumMap.App.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.Mobile.Services
{
    /// <summary>
    /// Implementation of the device information.
    /// </summary>
    public class MobileAppDeviceInfo : IAppDeviceInfo
    {
        /// <summary>
        /// True if the application is currently hosted in a browser.
        /// </summary>
        public bool IsBrowserHosted
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the current platform type.
        /// </summary>
        public DevicePlatformType Platform
        {
            get
            {
                if(DeviceInfo.Platform == DevicePlatform.Android)
                {
                    return DevicePlatformType.Android;
                }
                else if(DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    return DevicePlatformType.iOS;
                }
                else if (DeviceInfo.Platform == DevicePlatform.WinUI)
                {
                    return DevicePlatformType.Windows;
                }
                else
                {
                    return DevicePlatformType.Unknown;
                }
            }
        }

        /// <summary>
        /// Provides access to the system theme settings.
        /// </summary>
        public Notl.MuseumMap.App.Common.AppTheme SystemTheme 
        { 
            get
            {
                var systemTheme = Application.Current?.RequestedTheme;
                if(systemTheme == null)
                {
                    return Notl.MuseumMap.App.Common.AppTheme.Light;
                }

                switch(systemTheme)
                {
                    case Microsoft.Maui.ApplicationModel.AppTheme.Dark:
                        return Notl.MuseumMap.App.Common.AppTheme.Dark;
                    case Microsoft.Maui.ApplicationModel.AppTheme.Light:
                    default:
                        return Notl.MuseumMap.App.Common.AppTheme.Light;
                }
            }
        }

        /// <summary>
        /// Gets the current location and calls back the handler.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task GetCurrentLocationAsync(Func<Notl.MuseumMap.App.Common.Location?, Task> callback, TimeSpan timeout)
        {
            // Check/prompt for permission first.
            //var status = await CheckAndRequestLocationPermission();

            //// If access is granted, check the location.
            //if (status == PermissionStatus.Granted)
            //{
                try
                {
                    var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Default, timeout));
                    if (location != null)
                    {
                        await callback.Invoke(new Notl.MuseumMap.App.Common.Location { Latitude = location.Latitude, Longitude = location.Longitude });
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            //}

            // If no location was found, return null.
            await callback.Invoke(null);
        }


    }
}
