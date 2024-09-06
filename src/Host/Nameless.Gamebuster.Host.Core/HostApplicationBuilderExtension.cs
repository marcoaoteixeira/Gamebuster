using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Nameless.Gamebuster.Host.Core;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class HostApplicationBuilderExtension {
    private const string OPEN_TELEMETRY_PROTOCOL_KEY = "OTEL_EXPORTER_OTLP_ENDPOINT";
    private const string APPLICATION_INSIGHTS_CONNECTION_STRING_KEY = "APPLICATIONINSIGHTS_CONNECTION_STRING";

    public static IHostApplicationBuilder ConfigureAspireHost(this IHostApplicationBuilder builder) {
        builder.ConfigureOpenTelemetry();

        builder.ConfigureHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(httpClientBuilder => {
            // Turn on resilience by default
            httpClientBuilder.AddStandardResilienceHandler();

            // Turn on service discovery by default
            httpClientBuilder.AddServiceDiscovery();
        });

        return builder;
    }

    private static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder) {
        builder.Logging.AddOpenTelemetry(logging => {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics => {
                metrics.AddAspNetCoreInstrumentation()
                       .AddHttpClientInstrumentation()
                       .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing => {
                tracing.AddAspNetCoreInstrumentation()
                    // Uncomment the following line to enable gRPC instrumentation
                    // (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.ConfigureOpenTelemetryExporters();

        return builder;
    }

    private static IHostApplicationBuilder ConfigureOpenTelemetryExporters(this IHostApplicationBuilder builder) {
        // this configuration comes from environment.
        var useOpenTelemetryProtocol = !string.IsNullOrWhiteSpace(builder.Configuration[OPEN_TELEMETRY_PROTOCOL_KEY]);

        if (useOpenTelemetryProtocol) {
            builder.Services
                   .AddOpenTelemetry()
                   .UseOtlpExporter();
        }

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration[APPLICATION_INSIGHTS_CONNECTION_STRING_KEY])) {
        //    builder.Services
        //           .AddOpenTelemetry()
        //           .UseAzureMonitor();
        //}

        return builder;
    }

    private static IHostApplicationBuilder ConfigureHealthChecks(this IHostApplicationBuilder builder) {
        builder.Services
               .AddHealthChecks()
               // Add a default liveness check to ensure app is responsive
               .AddCheck(name: "self",
                         check: () => HealthCheckResult.Healthy(),
                         tags: ["live"]);

        return builder;
    }
}
