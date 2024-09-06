using MediatR;
using Nameless.Gamebuster.Catalog.Objects.Requests;
using Nameless.Gamebuster.Catalog.Objects.Responses;

namespace Nameless.Gamebuster.Catalog.App.Queries;

public sealed record SearchGamesQuery : IRequest<SearchGamesResponse> {
    public SearchGamesRequest Request { get; }

    public SearchGamesQuery(SearchGamesRequest request) {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }
}
