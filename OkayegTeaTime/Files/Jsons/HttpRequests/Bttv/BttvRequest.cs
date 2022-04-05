using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.HttpRequests.Bttv;

public class BttvRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("bots")]
    public List<string> Bots { get; set; }

    [JsonPropertyName("channelEmotes")]
    public List<BttvEmote> ChannelEmotes { get; set; }

    [JsonPropertyName("sharedEmotes")]
    public List<BttvEmote> SharedEmotes { get; set; }
}
