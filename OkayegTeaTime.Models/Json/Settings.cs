using System.Collections.Immutable;

namespace OkayegTeaTime.Models.Json;

public sealed class Settings
{
    public required TwitchSettings Twitch { get; init; }

    public required DiscordSettings Discord { get; init; }

    public required ImmutableArray<string> OfflineChatEmotes { get; init; }

    public required SpotifySettings Spotify { get; init; }

    public required UserLists UserLists { get; init; }

    public required string RepositoryUrl { get; init; }

    public required string OfflineChatChannel { get; init; }

    public required DatabaseConnection DatabaseConnection { get; init; }

    public required string OpenWeatherMapApiKey { get; init; }
}
