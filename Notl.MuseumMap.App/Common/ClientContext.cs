﻿using Microsoft.AspNetCore.Components;
using Notl.MuseumMap.App.Services;

namespace Notl.MuseumMap.App.Common
{
    /// <summary>
    /// Manages interactions with the backend.
    /// </summary>
    public class ClientContext
    {
        readonly ClientContextOptions options;

        /// <summary>
        /// Constructs the Admin Context with the required dependencies.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="httpClient"></param>
        public ClientContext(ClientContextOptions options, HttpClient httpClient, IAppDeviceInfo appDeviceInfo, IAppSettings appSettings, IAppVersionInfo appVersionInfo)
        {
            this.options = options;

            Http = httpClient;
            Maps = new MapClient(options.MuseumMapServiceBaseUrl, httpClient);
            AppSettings = appSettings;
            DeviceInfo = appDeviceInfo;
            VersionInfo = appVersionInfo;
        }

        /// <summary>
        /// Access to the backend Admin API for the Download Service.
        /// </summary>
        public MapClient Maps { get; set; }

        /// <summary>
        /// Access to the App Settings.
        /// </summary>
        public IAppSettings AppSettings { get; set; }

        /// <summary>
        /// Access to device information.
        /// </summary>
        public IAppDeviceInfo DeviceInfo { get; set; }

        /// <summary>
        /// Access to version information.
        /// </summary>
        public IAppVersionInfo VersionInfo { get; set; }

        /// <summary>
        /// Provides access to the underlying HTTP Client.
        /// </summary>
        public HttpClient Http { get; set; }

        /// <summary>
        /// Gets or sets the user theme information (also uses system setting as default).
        /// </summary>
        public AppTheme Theme
        {
            get
            {
                if (AppSettings.ContainKey(nameof(Theme)))
                {
                    if (Enum.TryParse(AppSettings.GetItem<string>(nameof(Theme)), out AppTheme result))
                    {
                        return result;
                    }
                }
                return DeviceInfo.SystemTheme;
            }
            set
            {
                AppSettings.SetItem(nameof(Theme), value.ToString());
            }
        }
    }
}
