using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using HLE.Numerics;
using HLE.Strings;
using OkayegTeaTime.Twitch.JsonConverters;

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

    internal static readonly Dictionary<EmoteImageFormats, string> _imageFormatValues = new()
    {
        { EmoteImageFormats.Static, "static" },
        { EmoteImageFormats.Animated, "animated" }
    };

    internal static readonly Dictionary<EmoteScales, string> _scaleValues = new()
    {
        { EmoteScales.One, "1.0" },
        { EmoteScales.Two, "2.0" },
        { EmoteScales.Three, "3.0" }
    };

    internal static readonly Dictionary<EmoteThemes, string> _themeValues = new()
    {
        { EmoteThemes.Light, "light" },
        { EmoteThemes.Dark, "dark" }
    };

    public bool TryGetImageUrl(EmoteImageFormats format, EmoteThemes theme, EmoteScales scale, [MaybeNullWhen(false)] out string url)
    {
        url = null;
        ValueStringBuilder urlBuilder = stackalloc char[250];
        urlBuilder.Append("https://static-cdn.jtvnw.net/emoticons/v2/");
        if ((Formats & format) != format || !NumberHelper.IsOnlyOneBitSet((int)format))
        {
            return false;
        }

        urlBuilder.Append(Id);
        urlBuilder.Append('/');
        urlBuilder.Append(_imageFormatValues[format]);

        if ((Themes & theme) != theme || !NumberHelper.IsOnlyOneBitSet((int)theme))
        {
            return false;
        }

        urlBuilder.Append('/');
        urlBuilder.Append(_themeValues[theme]);

        if ((Scales & scale) != scale || !NumberHelper.IsOnlyOneBitSet((int)scale))
        {
            return false;
        }

        urlBuilder.Append('/');
        urlBuilder.Append(_scaleValues[scale]);
        url = StringPool.Shared.GetOrAdd(urlBuilder.WrittenSpan);
        return true;
    }

    public override string ToString()
    {
        return Name;
    }

    public bool Equals(Emote? other)
    {
        return ReferenceEquals(this, other) || Id == other?.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Emote other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Emote? left, Emote? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Emote? left, Emote? right)
    {
        return !(left == right);
    }
}