using System.Collections.Immutable;

namespace OkayegTeaTime.Models.Json;

public sealed class SpotifySettings
{
    public required string ClientId { get; init; }

    public required string ClientSecret { get; init; }

    public required string ChatPlaylistId { get; init; }

    public required ImmutableArray<long> ChatPlaylistUsers { get; init; }
}
