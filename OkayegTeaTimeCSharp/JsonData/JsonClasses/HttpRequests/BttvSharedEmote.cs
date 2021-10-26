using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.HttpRequests;

public class BttvSharedEmote
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("code")]
    public string Name { get; set; }

    [JsonPropertyName("imageType")]
    public string ImageType { get; set; }

    [JsonPropertyName("user")]
    public BttvUser User { get; set; }

    public override bool Equals(object obj)
    {
        return obj is BttvSharedEmote emote && emote.Name == Name;
    }

    public override string ToString()
    {
        return Name;
    }
}
