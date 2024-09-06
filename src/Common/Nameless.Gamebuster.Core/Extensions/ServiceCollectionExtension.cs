using System.Reflection;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nameless.Gamebuster.Behaviors;
using Nameless.Gamebuster.Infrastructure;
using Nameless.Gamebuster.Infrastructure.Impl;
using Nameless.Gamebuster.Jwt;
using Nameless.Gamebuster.Jwt.Impl;
using Nameless.Gamebuster.Validation;
using Nameless.Gamebuster.Validation.Impl;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nameless.Gamebuster;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtension {
    /// <summary>
    /// Adds cross-origin resource sharing (CORS) services.
    /// </summary>
    /// <param name="self">The <see cref="IServiceCollection"/> current instance.</param>
    /// <param name="configure">A configuration action if it needs to be configured.</param>
    /// <returns>The <see cref="IServiceCollection"/> current instance so other actions can be chained.</returns>
    public static IServiceCollection RegisterCors(this IServiceCollection self, Action<CorsOptions>? configure = null)
        => self.AddCors(configure ?? (_ => { }));

    /// <summary>
    /// Adds <see cref="IHttpContextAccessor"/> service to the application.
    /// </summary>
    /// <param name="self">The <see cref="IServiceCollection"/> current instance.</param>
    /// <returns>The <see cref="IServiceCollection"/> current instance so other actions can be chained.</returns>
    public static IServiceCollection RegisterHttpContextAccessor(this IServiceCollection self)
        => self.AddHttpContextAccessor();

    public static IHealthChecksBuilder RegisterHealthChecks(this IServiceCollection self)
        => self.AddHealthChecks();

    /// <summary>
    /// Adds authorization and authentication capabilities with JWT.
    /// </summary>
    /// <param name="self">The <see cref="IServiceCollection"/> current instance.</param>
    /// <param name="config">The <see cref="IConfiguration"/> instance.</param>
    /// <param name="configureAuthorization">A configuration authorization action if it needs to be configured.</param>
    /// <param name="configureJwt">A configuration JWT action if it needs to be configured.</param>
    /// <returns>The <see cref="IServiceCollection"/> current instance so other actions can be chained.</returns>
    public static IServiceCollection RegisterAuth(this IServiceCollection self, IConfiguration config, Action<AuthorizationOptions>? configureAuthorization = null, Action<JwtOptions>? configureJwt = null) {
        // we assume that the section was named "JwtOptions".
        var options = config.GetSection(nameof(JwtOptions))
                            .Get<JwtOptions>() ?? JwtOptions.Default;

        configureJwt?.Invoke(options);

        self.AddAuthorization(configureAuthorization ?? (_ => { }))
            .AddAuthentication(authenticationOptions => {
                authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                authenticationOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtBearerOptions => {
                jwtBearerOptions.RequireHttpsMetadata = options.RequireHttpsMetadata;
                jwtBearerOptions.SaveToken = options.SaveTokens;
                jwtBearerOptions.TokenValidationParameters = options.GetTokenValidationParameters();
                jwtBearerOptions.Events = new JwtBearerEvents {
                    OnAuthenticationFailed = ctx => {
                        if (ctx.Exception is SecurityTokenExpiredException) {
                            ctx.Response.Headers[Root.HttpResponseHeaders.X_JWT_EXPIRED] = bool.TrueString;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        self.AddSingleton<IJwtService, JwtService>(provider => new JwtService(options: Options.Create(options),
                                                                              clock: provider.GetService<ISystemClock>() ?? SystemClock.Instance,
                                                                              logger: provider.GetLogger<JwtService>()));

        return self;
    }

    /// <summary>
    /// Adds MediatR to the application.
    /// </summary>
    /// <param name="self">The <see cref="IServiceCollection"/> current instance.</param>
    /// <param name="supportAssemblies">A list of <see cref="Assembly"/> that will be used to register MediatR services.</param>
    /// <returns>The <see cref="IServiceCollection"/> current instance so other actions can be chained.</returns>
    public static IServiceCollection RegisterMediatR(this IServiceCollection self, params Assembly[] supportAssemblies)
        => self.AddMediatR(configure => {
            configure.RegisterServicesFromAssemblies(supportAssemblies);
            configure.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

    /// <summary>
    /// Registers all endpoints (minimal) for the application.
    /// </summary>
    /// <param name="self">The <see cref="IServiceCollection"/> current instance.</param>
    /// <param name="supportAssemblies">A list of <see cref="Assembly"/> that will be used to register MediatR services.</param>
    /// <returns>The <see cref="IServiceCollection"/> current instance so other actions can be chained.</returns>
    public static IServiceCollection RegisterEndpoints(this IServiceCollection self, IEnumerable<Assembly> supportAssemblies) {
        var assemblies = supportAssemblies.ToArray();
        if (assemblies.Length <= 0) {
            return self;
        }

        var endpoints = assemblies.SelectMany(assembly => assembly.SearchForImplementations<IMinimalEndpoint>());
        // NOTE: In the future, check if it'll be necessary to "keyed" this services.
        foreach (var endpoint in endpoints) {
            self.AddTransient(serviceType: typeof(IMinimalEndpoint),
                              implementationType: endpoint);
        }

        return self;
    }

    /// <summary>
    /// Adds services required to use options.
    /// </summary>
    /// <param name="self">The <see cref="IServiceCollection"/> current instance.</param>
    /// <returns>The <see cref="IServiceCollection"/> current instance so other actions can be chained.</returns>
    public static IServiceCollection RegisterOptions(this IServiceCollection self)
        => self.AddOptions();

    /// <summary>
    /// Registers an <see cref="IConfiguration"/> instance which <see cref="TOptions"/> will bind against.
    /// </summary>
    /// <typeparam name="TOptions">Type of the option.</typeparam>
    /// <param name="self">The <see cref="IServiceCollection"/> current instance.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The <see cref="IServiceCollection"/> current instance so other actions can be chained.</returns>
    public static IServiceCollection RegisterConfiguration<TOptions>(this IServiceCollection self, IConfiguration configuration)
        where TOptions : class
        => self.Configure<TOptions>(configuration.GetSection(typeof(TOptions).Name));

    /// <summary>
    /// Adds services required for failed requests that will end with <see cref="Microsoft.AspNetCore.Mvc.ProblemDetails"/>.
    /// </summary>
    /// <param name="self">The <see cref="IServiceCollection"/> current instance.</param>
    /// <param name="configure">Configuration action.</param>
    /// <returns>The <see cref="IServiceCollection"/> current instance so other actions can be chained.</returns>
    public static IServiceCollection RegisterProblemDetails(this IServiceCollection self, Action<ProblemDetailsOptions>? configure = null)
        => self.AddProblemDetails(configure);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="self">The <see cref="IServiceCollection"/> current instance.</param>
    /// <param name="configure">Configuration action.</param>
    /// <returns>The <see cref="IServiceCollection"/> current instance so other actions can be chained.</returns>
    public static IServiceCollection RegisterRouting(this IServiceCollection self, Action<RouteOptions>? configure = null)
        => self.AddRouting(configure ?? (_ => { }));

    /// <summary>
    /// Adds the <see cref="ISystemClock"/> services to the application.
    /// </summary>
    /// <param name="self">The <see cref="IServiceCollection"/> current instance.</param>
    /// <returns>The <see cref="IServiceCollection"/> current instance so other actions can be chained.</returns>
    public static IServiceCollection RegisterSystemClock(this IServiceCollection self)
        => self.AddSingleton(SystemClock.Instance);

    /// <summary>
    /// Adds Swagger and endpoint API explorer services.
    /// </summary>
    /// <param name="self">The <see cref="IServiceCollection"/> current instance.</param>
    /// <param name="configure">Configuration action.</param>
    /// <returns>The <see cref="IServiceCollection"/> current instance so other actions can be chained.</returns>
    public static IServiceCollection RegisterSwagger(this IServiceCollection self, Action<SwaggerGenOptions>? configure = null)
        => self.AddEndpointsApiExplorer()
               .AddSwaggerGen(configure ?? (_ => { }));

    /// <summary>
    /// Adds the <see cref="IValidationService"/> to the web application.
    /// </summary>
    /// <param name="self">The <see cref="IServiceCollection"/> current instance.</param>
    /// <param name="supportAssemblies">A list of <see cref="Assembly"/> that will be used be the service.</param>
    /// <returns>The <see cref="IServiceCollection"/> current instance so other actions can be chained.</returns>
    public static IServiceCollection RegisterValidationService(this IServiceCollection self, params Assembly[] supportAssemblies)
        => self.AddValidatorsFromAssemblies(supportAssemblies)
               .AddSingleton<IValidationService, ValidationService>();

    /// <summary>
    /// Adds API versioning services to the web application.
    /// </summary>
    /// <param name="self">The <see cref="IServiceCollection"/> current instance.</param>
    /// <returns>The <see cref="IServiceCollection"/> current instance so other actions can be chained.</returns>
    public static IServiceCollection RegisterVersioning(this IServiceCollection self) {
        self
            .AddApiVersioning(opts => {
                // Add the headers "api-supported-versions" and "api-deprecated-versions"
                // This is better for discoverability
                opts.ReportApiVersions = true;

                // AssumeDefaultVersionWhenUnspecified should only be enabled when supporting legacy services that did not previously
                // support API versioning. Forcing existing clients to specify an explicit API version for an
                // existing service introduces a breaking change. Conceptually, clients in this situation are
                // bound to some API version of a service, but they don't know what it is and never explicit request it.
                opts.AssumeDefaultVersionWhenUnspecified = true;
                opts.DefaultApiVersion = new ApiVersion(1);

                // Defines how an API version is read from the current HTTP request
                opts.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("api-version"),
                    new UrlSegmentApiVersionReader()
                );
            })
            .AddApiExplorer(opts => {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                opts.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                opts.SubstituteApiVersionInUrl = true;
            });

        return self;
    }
}
