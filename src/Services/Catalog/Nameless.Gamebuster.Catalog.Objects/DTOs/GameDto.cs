using System.Text.Json.Serialization;

namespace Nameless.Gamebuster.Catalog.Objects.DTOs;

public sealed record GameDto {
    [JsonPropertyName("id")]
    public int ID { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("background_image_url")]
    public string BackgroundImageUrl { get; init; } = string.Empty;

    [JsonPropertyName("rating")]
    public double Rating { get; init; }

    [JsonPropertyName("platforms")]
    public string[] Platforms { get; init; } = [];

    [JsonPropertyName("screenshots")]
    public string[] Screenshots { get; init; } = [];
}