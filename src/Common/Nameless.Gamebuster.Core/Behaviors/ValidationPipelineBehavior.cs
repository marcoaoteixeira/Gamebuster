using MediatR;
using Nameless.Gamebuster.Validation;

namespace Nameless.Gamebuster.Behaviors;

/// <summary>
/// Validates the requests that comes through MediatR request pipeline.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <param name="validationService">The validation service implementation.</param>
public sealed class ValidationPipelineBehavior<TRequest, TResponse>(IValidationService validationService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> {
    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        await validationService.ValidateAsync(value: request,
                                              throwOnFailure: true,
                                              cancellationToken: cancellationToken);
        return await next();
    }
}
