using Microsoft.OpenApi.Models;
using Notl.MuseumMap.Api.Tools;
using Notl.MuseumMap.Core.Common;
using Notl.MuseumMap.Core.Managers;
using Notl.MuseumMap.Core.Tools;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Azure.Monitor.OpenTelemetry.AspNetCore;

const string CorsPolicyName = "CorsPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(o => o.AddPolicy(CorsPolicyName, builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NOTL Museum Map Service API", Version = "v1" });
    var filePath = Path.Combine(AppContext.BaseDirectory, "Notl.MuseumMap.Api.xml");
    c.IncludeXmlComments(filePath);
    c.SchemaFilter<AutoRestSchemaFilter>();

    // Create a custom operation ID generator
    c.CustomOperationIds((e) =>
    {
        return $"{e.ActionDescriptor.RouteValues["action"]}";
    });

    var scheme = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        In = ParameterLocation.Header,
        Name = "Token Authentication",
        Description = "Azure Active Directory Token",
        Reference = new OpenApiReference
        {
            Id = "Token",
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(scheme.Reference.Id, scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {scheme, Array.Empty<string>() }
    });

});

// The following line enables Azure Monitor Distro.
builder.Services.AddAzureMonitor(o => 
    o.ConnectionString = builder.Configuration.GetValue<String>("AppInsightsConnectionString"));

builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<DbManagerOptions>();
builder.Services.AddSingleton<MuseumMapOptions>();
builder.Services.AddSingleton<DbManager>();
builder.Services.AddSingleton<StorageManager>();
builder.Services.AddSingleton<MapManager>();
builder.Services.AddSingleton<AdminManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolicyName);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
//app.UseAuthentication();
app.MapControllers();
app.Run();
