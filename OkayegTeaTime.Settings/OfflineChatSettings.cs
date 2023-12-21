using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace OkayegTeaTime.Settings;

public sealed class OfflineChatSettings : IEquatable<OfflineChatSettings>
{
    [RegularExpression(SettingsValidator.TwitchUsernamePattern)]
    public required string Channel { get; init; }

    [MinLength(1)]
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")] // source generator doesnt work with ImmutableArray<T>
    public required string[] Emotes { get; init; }

    [RegularExpression("(?i)^[a-z0-9]{22}$")]
    public required string ChatPlaylistId { get; init; }

    public required ImmutableArray<long> PlaylistUsers { get; init; }

    public required ImmutableArray<long> Users { get; init; }

    public bool Equals(OfflineChatSettings? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj) => obj is OfflineChatSettings other && Equals(other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(OfflineChatSettings? left, OfflineChatSettings? right) => Equals(left, right);

    public static bool operator !=(OfflineChatSettings? left, OfflineChatSettings? right) => !(left == right);
}
