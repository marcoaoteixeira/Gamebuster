using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Nameless.Gamebuster.Catalog.App.Extensions;
using Nameless.Gamebuster.Helpers;
using Nameless.Gamebuster.Host.Core;
using Nameless.RawgClient;

namespace Nameless.Gamebuster.Catalog.App;

public static class EntryPoint {
    private static readonly Assembly[] SupportAssemblies = [typeof(EntryPoint).Assembly];

    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Enable Aspire
        builder.ConfigureAspireHost();

        builder.Configuration
               .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"AppSettings.{builder.Environment.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables();

        builder.Logging
               .AddConfiguration(builder.Configuration.GetSection("Logging"))
               .AddConsole()
               .AddDebug();

        builder.Services
               .RegisterOptions()
               .RegisterValidationService(SupportAssemblies)
               .RegisterMediatR(SupportAssemblies)
               .RegisterHttpContextAccessor()
               .RegisterCors()
               .RegisterRouting()
               .RegisterAuth(builder.Configuration)
               .RegisterProblemDetails()
               .RegisterSystemClock()
               .RegisterVersioning()
               .RegisterEndpoints(SupportAssemblies)
               .RegisterSwagger()
               .RegisterRawgOptions(builder.Configuration)
               .RegisterRawgClient();

        var app = builder.Build();

        app.ResolveCors()
           .ResolveRouting()
           .ResolveEndpoints()
           .ResolveHealthChecks(enableHealthChecks: builder.Environment.IsDevelopment())
           .ResolveJwtAuth()
           .ResolveSwagger(configureUI: options => SwaggerConfigurationHelper.ConfigureVersioning(options, app.Services.GetRequiredService<IApiVersionDescriptionProvider>()))
           .ResolveHttpSecurity(enableHsts: builder.Environment.IsProduction())
           .ResolveErrorHandling();

        app.Run();
    }                    
}
