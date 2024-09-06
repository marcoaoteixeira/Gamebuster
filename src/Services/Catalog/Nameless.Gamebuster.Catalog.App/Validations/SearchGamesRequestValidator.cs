using FluentValidation;
using Nameless.Gamebuster.Catalog.App.Objects;

namespace Nameless.Gamebuster.Catalog.App.Validations;

public sealed class SearchGamesRequestValidator : AbstractValidator<SearchGamesRequest> {
    public SearchGamesRequestValidator() {
        RuleFor(request => request.PageNumber)
            .LessThanOrEqualTo(0);

        RuleFor(request => request.PageSize)
            .LessThanOrEqualTo(0);
    }
}
