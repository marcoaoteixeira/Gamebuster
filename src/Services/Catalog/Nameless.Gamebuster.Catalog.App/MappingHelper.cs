using Nameless.Gamebuster.Catalog.Objects.DTOs;
using Nameless.RawgClient.Objects;

namespace Nameless.Gamebuster.Catalog.App;

public static class MappingHelper {
    public static GameDto MapGame(Game game) {
        return new GameDto {
            ID = game.Id,
            Name = game.Name,
            Description = game.Description,
            BackgroundImageUrl = game.BackgroundImageUrl,
            Rating = game.Rating,
            Platforms = game.Platforms
                            .Select(platform => platform.Platform?.Name ?? string.Empty)
                            .ToArray(),
            Screenshots = game.ShortScreenshots
                              .Select(screenshot => screenshot.ImageUrl)
                              .ToArray(),
        };
    }
}
