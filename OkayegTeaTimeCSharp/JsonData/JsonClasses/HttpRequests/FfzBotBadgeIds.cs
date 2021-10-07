using System.Text.Json.Serialization;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.HttpRequests
{
    public class FfzBotBadgeIds
    {
        [JsonPropertyName("2")]
        public List<int> UserIds { get; set; }
    }
}
