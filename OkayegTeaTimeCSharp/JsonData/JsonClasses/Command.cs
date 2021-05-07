using Newtonsoft.Json;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class Command
    {
        [JsonProperty("CommandName")]
        public string CommandName { get; set; }

        [JsonProperty("Alias")]
        public List<string> Alias { get; set; }

        [JsonProperty("Parameter")]
        public List<string> Parameter { get; set; }

        [JsonProperty("Description")]
        public List<string> Description { get; set; }

        [JsonProperty("Cooldown")]
        public int Cooldown { get; set; }
    }
}
