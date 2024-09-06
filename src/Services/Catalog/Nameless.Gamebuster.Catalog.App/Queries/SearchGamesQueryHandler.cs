using MediatR;
using Nameless.Gamebuster.Catalog.Objects.Responses;
using Nameless.RawgClient;
using Nameless.RawgClient.Requests.Games;

namespace Nameless.Gamebuster.Catalog.App.Queries;

public sealed class SearchGamesQueryHandler : IRequestHandler<SearchGamesQuery, SearchGamesResponse> {
    private readonly IRawg _rawg;

    public SearchGamesQueryHandler(IRawg rawg) {
        _rawg = rawg ?? throw new ArgumentNullException(nameof(rawg));
    }

    public async Task<SearchGamesResponse> Handle(SearchGamesQuery request, CancellationToken cancellationToken) {
        var getGamesRequest = new GetGamesRequest {
            Search = request.Request.Query,
            PageNumber = request.Request.PageNumber,
            PageSize = request.Request.PageSize
        };
        var response = await _rawg.QueryAsync(getGamesRequest, cancellationToken);

        if (!response.Succeeded) {
            return new SearchGamesResponse {
                Error = response.Error
            };
        }

        return new SearchGamesResponse {
            Total = response.Count,
            Previous = response.Previous is not null
                ? request.Request with {
                    PageNumber = response.Previous.PageNumber.GetValueOrDefault()
                }
                : null,
            Next = response.Next is not null
                ? request.Request with {
                    PageNumber = response.Next.PageNumber.GetValueOrDefault()
                }
                : null,
            Results = response.Results
                              .Select(MappingHelper.MapGame)
                              .ToArray()
        };
    }
}
