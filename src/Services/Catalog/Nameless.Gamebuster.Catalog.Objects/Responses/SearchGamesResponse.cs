using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Nameless.Gamebuster.Catalog.Objects.DTOs;
using Nameless.Gamebuster.Catalog.Objects.Requests;

namespace Nameless.Gamebuster.Catalog.Objects.Responses;

public sealed record SearchGamesResponse {
    [JsonPropertyName("total")]
    public int Total { get; init; }

    [JsonPropertyName("results")]
    public GameDto[] Results { get; init; } = [];

    [JsonPropertyName("previous_request")]
    public SearchGamesRequest? Previous { get; init; }

    [JsonPropertyName("next_request")]
    public SearchGamesRequest? Next { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }

    [MemberNotNullWhen(returnValue: false, nameof(Error))]
    [JsonPropertyName("succeeded")]
    public bool Succeeded => Error is null;
}
