using Newtonsoft.Json;
using System.Collections.Generic;

public class UserList
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("matches")]
    public List<string> Matches { get; set; }
}
