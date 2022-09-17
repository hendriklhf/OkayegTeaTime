#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.Models;

public sealed class BttvRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("bots")]
    public string[] Bots { get; set; }

    [JsonPropertyName("channelEmotes")]
    public BttvEmote[] ChannelEmotes { get; set; }

    [JsonPropertyName("sharedEmotes")]
    public BttvEmote[] SharedEmotes { get; set; }
}
