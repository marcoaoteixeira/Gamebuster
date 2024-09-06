using System.Net;
using Microsoft.AspNetCore.Http;
using Nameless.Gamebuster.Jwt;

namespace Nameless.Gamebuster.Infrastructure;

/// <summary>
/// Middleware used to implement JWT authorization logic.
/// </summary>
public sealed class JwtAuthorizationMiddleware(RequestDelegate next, IJwtService jwtService) {
    private const string AUTH_KEY = nameof(HttpRequestHeader.Authorization);

    /// <summary>
    /// Handles the middleware invocation.
    /// </summary>
    /// <param name="context">Current <see cref="HttpContent"/> instance.</param>
    /// <returns>A <see cref="Task"/> representing the invocation execution.</returns>
    public async Task InvokeAsync(HttpContext context) {
        var header = context.Request
                            .Headers[AUTH_KEY]
                            .FirstOrDefault();

        if (header is not null) {
            var token = header.Split(Root.Separators.SPACE)
                              .Last();
            if (jwtService.TryValidate(token, out var principal)) {
                context.User = principal;
            }
        }

        await next(context);
    }
}
