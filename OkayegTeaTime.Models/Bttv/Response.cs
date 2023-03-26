using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Models.Bttv;

public sealed class Response
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("bots")]
    public string[] Bots { get; set; }

    [JsonPropertyName("channelEmotes")]
    public Emote[] ChannelEmotes { get; set; }

    [JsonPropertyName("sharedEmotes")]
    public Emote[] SharedEmotes { get; set; }
}
