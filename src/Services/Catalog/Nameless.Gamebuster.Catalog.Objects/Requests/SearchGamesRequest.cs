using System.Text.Json.Serialization;

namespace Nameless.Gamebuster.Catalog.Objects.Requests;

public sealed record SearchGamesRequest {
    [JsonPropertyName("query")]
    public string Query { get; init; } = string.Empty;

    [JsonPropertyName("page_number")]
    public int PageNumber { get; init; } = 1;

    [JsonPropertyName("page_size")]
    public int PageSize { get; init; } = 10;
}