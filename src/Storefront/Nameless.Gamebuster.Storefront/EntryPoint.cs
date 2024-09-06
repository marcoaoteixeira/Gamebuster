using System.Reflection;
using Nameless.Gamebuster.Host.Core;
using Nameless.Gamebuster.Storefront.Components;

namespace Nameless.Gamebuster.Storefront;

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

        // Add services to the container.
        builder.Services
               .RegisterOptions()
               .RegisterValidationService(SupportAssemblies)
               .RegisterMediatR(SupportAssemblies)
               .RegisterHttpContextAccessor()
               .RegisterCors()
               .RegisterAuth(builder.Configuration)
               .RegisterSystemClock()
               .AddRazorComponents()
               .AddInteractiveServerComponents();

        var app = builder.Build();

        app.ResolveCors()
           .ResolveHealthChecks(enableHealthChecks: builder.Environment.IsDevelopment())
           .ResolveJwtAuth()
           .ResolveHttpSecurity(enableHsts: builder.Environment.IsProduction())
           .ResolveErrorHandling();
        
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
        }
        app.UseHttpsRedirection()
           .UseStaticFiles()
           .UseAntiforgery();
        app.MapRazorComponents<App>()
           .AddInteractiveServerRenderMode();

        app.Run();
    }
}
