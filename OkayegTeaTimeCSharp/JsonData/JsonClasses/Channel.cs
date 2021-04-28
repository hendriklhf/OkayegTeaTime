using Newtonsoft.Json;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class Channel
    {
        [JsonProperty("Nname")]
        public string Name { get; set; }

        [JsonProperty("Cytube")]
        public string Cytube { get; set; }

        [JsonProperty("Beatsense")]
        public string Beatsense { get; set; }
    }
}
