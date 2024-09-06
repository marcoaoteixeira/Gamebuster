using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Gamebuster.Filters;
using Nameless.Gamebuster.Infrastructure;
using Nameless.Gamebuster.Validation;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Nameless.Gamebuster;

/// <summary>
/// <see cref="IApplicationBuilder"/> extension methods.
/// </summary>
public static class ApplicationBuilderExtension {
    /// <summary>
    /// Adds CORS middleware to web application pipeline to enable cross domain requests.
    /// </summary>
    /// <param name="self">The <see cref="IApplicationBuilder"/> current instance.</param>
    /// <param name="configure">A configuration action.</param>
    /// <returns>The current instance of <see cref="IApplicationBuilder"/> so other actions can be chained.</returns>
    public static IApplicationBuilder ResolveCors(this IApplicationBuilder self, Action<CorsPolicyBuilder>? configure = null)
        => self.UseCors(configure ?? (opts => opts.AllowAnyOrigin()
                                                  .AllowAnyMethod()
                                                  .AllowAnyHeader()));

    /// <summary>
    /// Adds a middleware to the pipeline that will catch exceptions, log them, and re-execute the request in an alternate pipeline.
    /// The request will not be re-executed if the response has already started.
    /// Also enable validation treatment for request objects on endpoint execution.
    /// </summary>
    /// <param name="self">The <see cref="IApplicationBuilder"/> current instance.</param>
    /// <param name="enableValidationExceptionTreatment">Enables endpoint request objects validation.</param>
    /// <returns>The current instance of <see cref="IApplicationBuilder"/> so other actions can be chained.</returns>
    public static IApplicationBuilder ResolveErrorHandling(this IApplicationBuilder self, bool enableValidationExceptionTreatment = true) {
        return self.UseExceptionHandler(enableValidationExceptionTreatment ? ValidationExceptionTreatment : _ => { });

        static void ValidationExceptionTreatment(IApplicationBuilder builder)
            => builder.Run(ctx => TryHandleValidationException(ctx, out var result)
                               ? result.ExecuteAsync(ctx)
                               : Results.Problem()
                                        .ExecuteAsync(ctx));
    }

    /// <summary>
    /// Initializes health checks endpoints and services.
    /// <br /><br />
    /// <strong>Attention:</strong> Adding health checks endpoints to applications in non-development environments
    /// has security implications. See <a href="https://aka.ms/dotnet/aspire/healthchecks">Aspire Health Checks</a> for details
    /// before enabling these endpoints in non-development environments.
    /// </summary>
    /// <param name="self">The <see cref="IApplicationBuilder"/> current instance.</param>
    /// <param name="enableHealthChecks">Set <c>true</c> to enable health checks.</param>
    /// <returns>The current instance of <see cref="IApplicationBuilder"/> so other actions can be chained.</returns>
    public static IApplicationBuilder ResolveHealthChecks(this IApplicationBuilder self, bool enableHealthChecks = false) {
        if (!enableHealthChecks) { return self; }

        // All health checks must pass for app to be considered ready to accept traffic after starting
        self.UseHealthChecks("/health");

        // Only health checks tagged with the "live" tag must pass for app to be considered alive
        self.UseHealthChecks("/alive", new HealthCheckOptions {
            Predicate = registration => registration.Tags.Contains("live")
        });

        return self;
    }

    /// <summary>
    /// Initializes support for HSTS and HTTP redirection to HTTPS.
    /// </summary>
    /// <param name="self">The <see cref="IApplicationBuilder"/> current instance.</param>
    /// <param name="enableHsts">Enable HSTS.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> current instance so other actions can be chained.</returns>
    public static IApplicationBuilder ResolveHttpSecurity(this IApplicationBuilder self, bool enableHsts = false) {
        if (enableHsts) {
            // The default HSTS value is 30 days. You may want to change
            // this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            self.UseHsts();
        }

        self.UseHttpsRedirection();

        return self;
    }

    /// <summary>
    /// Initializes authentication, authorization and JWT auth services.
    /// </summary>
    /// <param name="self">The <see cref="IApplicationBuilder"/> current instance.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> current instance so other actions can be chained.</returns>
    public static IApplicationBuilder ResolveJwtAuth(this IApplicationBuilder self)
        => self.UseAuthorization()
               .UseAuthentication()
               .UseMiddleware<JwtAuthorizationMiddleware>();

    /// <summary>
    /// Initializes the endpoint (minimal) service.
    /// This call must be preceded by a call to <see cref="ResolveRouting"/>.
    /// </summary>
    /// <param name="self">The <see cref="IApplicationBuilder"/> instance.</param>
    /// <param name="useEndpointValidation">Whether it should use endpoint validation or not.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> instance.</returns>
    public static IApplicationBuilder ResolveEndpoints(this IApplicationBuilder self, bool useEndpointValidation = true)
        => self.UseEndpoints(builder => {
            var endpoints = builder.ServiceProvider
                                   .GetRequiredService<IEnumerable<IMinimalEndpoint>>();

            foreach (var endpoint in endpoints) {
                var conventionBuilder = endpoint
                                        .Map(builder)
                                        .WithOpenApi()
                                        .WithName(endpoint.Name)
                                        .WithSummary(endpoint.Summary)
                                        .WithDescription(endpoint.Description)
                                        .WithApiVersionSet(builder.NewApiVersionSet(endpoint.Group)
                                                                  .Build())
                                        .HasApiVersion(endpoint.Version);

                if (useEndpointValidation) {
                    conventionBuilder.AddEndpointFilter(new ValidateEndpointFilter());
                }
            }
        });

    /// <summary>
    /// Resolves the routing service. This call must be followed by a call to <see cref="ResolveEndpoints"/>.
    /// </summary>
    /// <param name="self">The application builder instance.</param>
    /// <returns>The application builder instance.</returns>
    public static IApplicationBuilder ResolveRouting(this IApplicationBuilder self)
        => self.UseRouting();

    /// <summary>
    /// Initializes Swagger services.
    /// </summary>
    /// <param name="self">The <see cref="IApplicationBuilder"/> instance.</param>
    /// <param name="configure">Configure Swagger service.</param>
    /// <param name="configureUI">Configure Swagger UI.</param>
    /// <returns></returns>
    public static IApplicationBuilder ResolveSwagger(this IApplicationBuilder self, Action<SwaggerOptions>? configure = null, Action<SwaggerUIOptions>? configureUI = null)
        => self.UseSwagger(configure ?? (_ => { }))
               .UseSwaggerUI(configureUI ?? (_ => { }));

    private static TException? GetExceptionFromHttpContext<TException>(HttpContext httpContext)
        where TException : Exception {
        var feature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
        return feature?.Error as TException;
    }

    private static bool TryHandleValidationException(HttpContext ctx, [NotNullWhen(returnValue: true)] out IResult? result) {
        result = null;

        var ex = GetExceptionFromHttpContext<ValidationException>(ctx);
        if (ex is null) {
            return false;
        }

        result = Results.ValidationProblem(errors: ex.Errors.ToProblems(),
                                           detail: ex.Message);

        return true;
    }
}
