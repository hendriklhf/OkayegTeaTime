using System.Text.Json.Serialization;
using OkayegTeaTimeCSharp.HttpRequests;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.HttpRequests
{
    public class FfzSets
    {
        [JsonPropertyName(HttpRequest.FfzSetIdReplacement)]
        public FfzMainSet EmoteSet { get; set; }
    }
}
