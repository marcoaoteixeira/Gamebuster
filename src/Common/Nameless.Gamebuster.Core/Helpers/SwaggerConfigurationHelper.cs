using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Nameless.Gamebuster.Helpers {
    public static class SwaggerConfigurationHelper {
        public static void ConfigureJwtAuth(SwaggerGenOptions options) {
            var openApiSecurityScheme = new OpenApiSecurityScheme {
                In = ParameterLocation.Header,
                Description = "Enter JSON Web Token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower()
            };
            options.AddSecurityDefinition(
                name: JwtBearerDefaults.AuthenticationScheme,
                securityScheme: openApiSecurityScheme
            );

            var securityRequirement = new OpenApiSecurityRequirement {
                {
                    // key
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    // value
                    []
                }
            };
            options.AddSecurityRequirement(securityRequirement);
        }

        public static void ConfigureVersioning(SwaggerUIOptions options, IApiVersionDescriptionProvider versioning) {
            foreach (var description in versioning.ApiVersionDescriptions) {
                options.SwaggerEndpoint(
                    url: $"/swagger/{description.GroupName}/swagger.json",
                    name: description.GroupName.ToUpperInvariant()
                );
            }
        }
    }
}
