using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Gamebuster.Validation;

namespace Nameless.Gamebuster.Filters;

/// <summary>
/// Implementation of <see cref="IEndpointFilter"/> to work with <see cref="IValidationService"/>.
/// </summary>
public sealed class ValidateEndpointFilter : IEndpointFilter {
    /// <inheritdoc />
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
        var validationService = context.HttpContext.RequestServices.GetService<IValidationService>();
        if (validationService is null) {
            return await next(context);
        }

        var args = context.Arguments
                          .Where(ValidateAttribute.Present);
        var cancellationToken = context.HttpContext.RequestAborted;

        foreach (var arg in args) {
            if (arg is null) {
                continue;
            }

            var result = await validationService.ValidateAsync(arg, throwOnFailure: false, cancellationToken);

            if (!result.Succeeded) {
                return Results.ValidationProblem(result.ToProblems());
            }
        }

        return await next(context);
    }
}
