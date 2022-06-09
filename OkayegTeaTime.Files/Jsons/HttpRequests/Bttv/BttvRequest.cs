using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.HttpRequests.Bttv;

public class BttvRequest
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("bots")] public string[] Bots { get; set; }

    [JsonPropertyName("channelEmotes")] public BttvEmote[] ChannelEmotes { get; set; }

    [JsonPropertyName("sharedEmotes")] public BttvEmote[] SharedEmotes { get; set; }
}
