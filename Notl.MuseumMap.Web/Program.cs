using Blazored.LocalStorage;
using BlazorPanzoom;
using Majorsoft.Blazor.Components.Common.JsInterop;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Notl.MuseumMap.App;
using Notl.MuseumMap.App.Common;
using Notl.MuseumMap.Web;
using Notl.MuseumMap.Web.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Determine the environment settings from the Browser URL
string environment = EnvironmentDetector.Detect(builder.Configuration, builder.HostEnvironment.BaseAddress);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
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
builder.Services.AddSingleton<HttpClient>();

// Setup the Application insights.
//builder.Services.AddBlazorApplicationInsights(async applicationInsights =>
//{
//    await applicationInsights.SetInstrumentationKey(environmentConfig.InstrumentationKey ?? string.Empty);
//    await applicationInsights.TrackPageView();

//    var telemetryItem = new TelemetryItem
//    {
//        IKey = environmentConfig.InstrumentationKey,
//        Tags = new Dictionary<string, object>
//        {
//            {"ai.cloud.role", "AttendeeApp"},
//            {"ai.cloud.roleInstance", "Web" }
//        }
//    };

//    await applicationInsights.AddTelemetryInitializer(telemetryItem);
//});

// Setup the mud services.
builder.Services.AddMudServices();
builder.Services.AddLocalization();

// Setup the pan zoom service
builder.Services.AddBlazorPanzoomServices();

// Setup the app specific services.
builder.Services.AddScoped<IAppVersionInfo, BrowserAppVersionInfo>();
builder.Services.AddScoped<IAppDeviceInfo, BrowserAppDeviceInfo>();
builder.Services.AddScoped<IAppSettings, BrowserAppSettings>();
builder.Services.AddScoped((s) => new ClientContextOptions(builder.Configuration, environment));
builder.Services.AddScoped<ClientContext>();

await builder.Build().RunAsync();
