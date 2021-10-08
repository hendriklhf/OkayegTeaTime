using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.HttpRequests
{
    public class SevenTvEmoteSize
    {
        [JsonPropertyName("0")]
        public int Zero { get; set; }

        [JsonPropertyName("1")]
        public int One { get; set; }

        [JsonPropertyName("2")]
        public int Two { get; set; }

        [JsonPropertyName("3")]
        public int Three { get; set; }
    }
}
