using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Nameless.Gamebuster.Catalog.Objects.Responses;

namespace Nameless.Gamebuster.Storefront.Core.Queries.Catalog {
    public sealed class SearchGamesQueryHandler : IRequestHandler<SearchGamesQuery, SearchGamesResponse> {


        public Task<SearchGamesResponse> Handle(SearchGamesQuery request, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}
