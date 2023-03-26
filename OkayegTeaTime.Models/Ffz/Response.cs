#nullable disable

using System.Text.Json.Serialization;

namespace OkayegTeaTime.Models.Ffz;

public sealed class Response
{
    [JsonPropertyName("room")]
    public Room Room { get; set; }

    [JsonPropertyName("sets")]
    public Sets Set { get; set; }
}
