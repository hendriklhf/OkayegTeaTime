using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;

namespace OkayegTeaTime.Configuration;

public sealed class Settings : IEquatable<Settings>
{
    [Required]
    [ValidateObjectMembers]
    public required TwitchSettings Twitch { get; init; }

    [Required]
    [ValidateObjectMembers]
    public required DatabaseSettings Database { get; init; }

    [Required]
    [ValidateObjectMembers]
    public required Users Users { get; init; }

    [ValidateObjectMembers]
    public SpotifySettings? Spotify { get; init; }

    [ValidateObjectMembers]
    public OpenWeatherMapSettings? OpenWeatherMap { get; init; }

    [ValidateObjectMembers]
    public OfflineChatSettings? OfflineChat { get; init; }

    [Url]
    public required string RepositoryUrl { get; init; }

    public bool Equals(Settings? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj) => obj is Settings other && Equals(other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(Settings? left, Settings? right) => Equals(left, right);

    public static bool operator !=(Settings? left, Settings? right) => !(left == right);
}
