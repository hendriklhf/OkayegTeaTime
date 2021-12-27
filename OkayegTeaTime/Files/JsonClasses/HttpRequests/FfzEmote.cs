using System.Text.Json.Serialization;

namespace OkayegTeaTime.Files.JsonClasses.HttpRequests;

public class FfzEmote
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("public")]
    public bool IsPublic { get; set; }

    [JsonPropertyName("hidden")]
    public bool IsHidden { get; set; }

    [JsonPropertyName("modifier")]
    public bool IsModifier { get; set; }

    [JsonPropertyName("offset")]
    public object Offset { get; set; }

    [JsonPropertyName("margins")]
    public object Margins { get; set; }

    [JsonPropertyName("css")]
    public object Css { get; set; }

    [JsonPropertyName("owner")]
    public FfzOwner Owner { get; set; }

    [JsonPropertyName("urls")]
    public FfzUrls EmoteUrls { get; set; }

    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("usage_count")]
    public int UsageCount { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }

    [JsonPropertyName("last_updated")]
    public string LastUpdated { get; set; }

    public override bool Equals(object obj)
    {
        return obj is FfzEmote emote && emote.Id == Id;
    }

    public override string ToString()
    {
        return Name;
    }
}
