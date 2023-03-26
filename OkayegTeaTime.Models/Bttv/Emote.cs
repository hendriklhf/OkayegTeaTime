using System.Text.Json.Serialization;

#nullable disable
#pragma warning disable CS0659

namespace OkayegTeaTime.Models.Bttv;

public sealed class Emote
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("code")]
    public string Name { get; set; }

    [JsonPropertyName("imageType")]
    public string ImageType { get; set; }

    [JsonPropertyName("user")]
    public User User { get; set; }

#nullable enable
    public override bool Equals(object? obj)
    {
        return obj is Emote emote && emote.Id == Id;
    }

    public override string ToString()
    {
        return Name;
    }
}
