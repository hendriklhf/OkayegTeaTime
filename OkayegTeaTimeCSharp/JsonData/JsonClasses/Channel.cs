using Newtonsoft.Json;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class Channel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("cytube")]
        public string Cytube { get; set; }

        [JsonProperty("beatsense")]
        public string Beatsense { get; set; }
    }
}
