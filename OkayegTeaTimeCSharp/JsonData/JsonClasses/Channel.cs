using Newtonsoft.Json;

public class Channel
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("cytube")]
    public string Cytube { get; set; }

    [JsonProperty("beatsense")]
    public string Beatsense { get; set; }
}
