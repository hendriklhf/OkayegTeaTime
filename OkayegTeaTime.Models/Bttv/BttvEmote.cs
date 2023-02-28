#nullable disable
using System.Text.Json.Serialization;

#pragma warning disable CS0659

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace OkayegTeaTime.Models.Bttv;

public sealed class BttvEmote
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("code")]
    public string Name { get; set; }

    [JsonPropertyName("imageType")]
    public string ImageType { get; set; }

    [JsonPropertyName("user")]
    public BttvUser User { get; set; }

#nullable enable
    public override bool Equals(object? obj)
    {
        return obj is BttvEmote emote && emote.Id == Id;
    }

    public override string ToString()
    {
        return Name;
    }
}
