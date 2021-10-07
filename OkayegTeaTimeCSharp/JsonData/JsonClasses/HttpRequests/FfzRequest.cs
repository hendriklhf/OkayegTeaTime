using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.HttpRequests
{
    public class FfzRequest
    {
        [JsonPropertyName("room")]
        public FfzRoom Room { get; set; }

        [JsonPropertyName("sets")]
        public FfzSets Set { get; set; }
    }
}
