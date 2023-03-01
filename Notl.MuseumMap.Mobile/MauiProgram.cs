using Blazored.LocalStorage;
using BlazorPanzoom;
using Majorsoft.Blazor.Components.Common.JsInterop;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using Notl.MuseumMap.App.Common;
using Notl.MuseumMap.Mobile.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Notl.MuseumMap.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

            // Setup the mud services.
            builder.Services.AddMudServices();
            builder.Services.AddBlazorPanzoomServices();
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddJsInteropExtensions();
            builder.Services.AddBlazoredLocalStorage(config =>
            {
                config.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                config.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                config.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
                config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                config.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                config.JsonSerializerOptions.WriteIndented = false;
            });


            builder.Services.AddLocalization();

            // Setup the app specific services.
            builder.Services.AddScoped<IAppVersionInfo, MobileAppVersionInfo>();
            builder.Services.AddScoped<IAppDeviceInfo, MobileAppDeviceInfo>();
            builder.Services.AddScoped<IAppSettings, MobileAppSettings>();
            builder.Services.AddScoped((s) => new ClientContextOptions(EnvironmentSelector.Current.BaseUrl));
            builder.Services.AddScoped<ClientContext>();
            return builder.Build();
        }
    }
}