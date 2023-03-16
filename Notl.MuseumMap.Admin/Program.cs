using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Notl.MuseumMap.Admin;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Blazored.LocalStorage;
using MudBlazor.Services;
using Notl.MuseumMap.Admin.Common;
using Majorsoft.Blazor.Components.Common.JsInterop;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorPanzoom;
using MudBlazor;

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


builder.Services.AddScoped(sp => 
{
    var authorizationMessageHandler = sp.GetRequiredService<AuthorizationMessageHandler>();
    authorizationMessageHandler.InnerHandler = new HttpClientHandler();
    authorizationMessageHandler = authorizationMessageHandler.ConfigureHandler(
        authorizedUrls: new[] { builder.Configuration[$"{environment}:MuseumMapService:BaseUrl"] },
        scopes: new[] { builder.Configuration[$"{environment}:MuseumMapService:Scopes"] });
    return new HttpClient(authorizationMessageHandler)
    {
        BaseAddress = new Uri(builder.Configuration[$"{environment}:MuseumMapService:BaseUrl"] ?? string.Empty)
    };
});

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind($"{environment}:AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add(builder.Configuration[$"{environment}:MuseumMapService:Scopes"]);
});

// Setup the mud services.
builder.Services.AddMudServices();
builder.Services.AddMudMarkdownServices();
builder.Services.AddLocalization();

// Set up panzoom
builder.Services.AddBlazorPanzoomServices();

// Setup the app specific services.
builder.Services.AddScoped<IAppVersionInfo, BrowserAppVersionInfo>();
builder.Services.AddScoped<IAppDeviceInfo, BrowserAppDeviceInfo>();
builder.Services.AddScoped<IAppSettings, BrowserAppSettings>();
builder.Services.AddScoped((s) => new ClientContextOptions(builder.Configuration, environment));
builder.Services.AddScoped<ClientContext>();

await builder.Build().RunAsync();
