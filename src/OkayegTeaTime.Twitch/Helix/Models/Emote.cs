using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.Json.Serialization;
using HLE.Text;
using OkayegTeaTime.Twitch.Json.Converters;

namespace OkayegTeaTime.Twitch.Helix.Models;

// ReSharper disable once UseNameofExpressionForPartOfTheString
[DebuggerDisplay("\"{Name}\"")]
public class Emote : IEquatable<Emote>
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("format")]
    [JsonConverter(typeof(EmoteImageFormatConverter))]
    public required EmoteImageFormats Formats { get; init; }

    [JsonPropertyName("scale")]
    [JsonConverter(typeof(EmoteScaleJsonConverter))]
    public required EmoteScales Scales { get; init; }

    [JsonPropertyName("theme_mode")]
    [JsonConverter(typeof(EmoteThemeJsonConverter))]
    public required EmoteThemes Themes { get; init; }

    internal static readonly Dictionary<EmoteImageFormats, string> s_imageFormatValues = new()
    {
        { EmoteImageFormats.Static, "static" },
        { EmoteImageFormats.Animated, "animated" }
    };

    internal static readonly Dictionary<EmoteScales, string> s_scaleValues = new()
    {
        { EmoteScales.One, "1.0" },
        { EmoteScales.Two, "2.0" },
        { EmoteScales.Three, "3.0" }
    };

    internal static readonly Dictionary<EmoteThemes, string> s_themeValues = new()
    {
        { EmoteThemes.Light, "light" },
        { EmoteThemes.Dark, "dark" }
    };

    public bool TryGetImageUrl(EmoteImageFormats format, EmoteThemes theme, EmoteScales scale, [MaybeNullWhen(false)] out string url)
    {
        url = null;
        using ValueStringBuilder urlBuilder = new(stackalloc char[256]);
        urlBuilder.Append("https://static-cdn.jtvnw.net/emoticons/v2/");
        if ((Formats & format) != format || !BitOperations.IsPow2((int)format))
        {
            return false;
        }

        urlBuilder.Append(Id);
        urlBuilder.Append('/');
        urlBuilder.Append(s_imageFormatValues[format]);

        if ((Themes & theme) != theme || !BitOperations.IsPow2((int)theme))
        {
            return false;
        }

        urlBuilder.Append('/');
        urlBuilder.Append(s_themeValues[theme]);

        if ((Scales & scale) != scale || !BitOperations.IsPow2((int)scale))
        {
            return false;
        }

        urlBuilder.Append('/');
        urlBuilder.Append(s_scaleValues[scale]);
        url = StringPool.Shared.GetOrAdd(urlBuilder.WrittenSpan);
        return true;
    }

    public override string ToString() => Name;

    public bool Equals(Emote? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj) => obj is Emote other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Id, Name);

    public static bool operator ==(Emote? left, Emote? right) => Equals(left, right);

    public static bool operator !=(Emote? left, Emote? right) => !(left == right);
}
