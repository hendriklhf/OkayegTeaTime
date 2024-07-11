using System;
using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Json.Converters;

namespace OkayegTeaTime.Twitch.Helix.Models;

public readonly struct AccessToken : IEquatable<AccessToken>
{
    [JsonPropertyName("access_token")]
    public string Token { get; init; } = string.Empty;

    [JsonPropertyName("expires_in")]
    [JsonConverter(typeof(TimeOfExpirationJsonConverter))]
    public DateTime TimeOfExpiration { get; init; }

    [JsonIgnore]
#pragma warning disable S6354
    public bool IsValid => DateTime.UtcNow + TimeSpan.FromSeconds(30) < TimeOfExpiration;
#pragma warning restore S6354

    public static AccessToken Empty => new();

    public AccessToken()
    {
    }

    public bool Equals(AccessToken other) => TimeOfExpiration == other.TimeOfExpiration && Token == other.Token;

    public override bool Equals(object? obj) => obj is AccessToken other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(TimeOfExpiration, Token);

    public override string ToString() => Token;

    public static bool operator ==(AccessToken left, AccessToken right) => left.Equals(right);

    public static bool operator !=(AccessToken left, AccessToken right) => !(left == right);
}
