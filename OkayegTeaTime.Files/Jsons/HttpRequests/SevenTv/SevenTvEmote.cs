using System.Text.Json.Serialization;

#nullable disable

namespace OkayegTeaTime.Files.Jsons.HttpRequests.SevenTv;

public class SevenTvEmote
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("status")] public int Status { get; set; }

    [JsonPropertyName("visibility")] public int Visibility { get; set; }

    [JsonPropertyName("width")] public int[] Width { get; set; }

    [JsonPropertyName("height")] public int[] Height { get; set; }

#nullable enable
    public override bool Equals(object? obj)
    {
        return obj is SevenTvEmote emote && emote.Id == Id;
    }

    public override string ToString()
    {
        return Name;
    }
}
