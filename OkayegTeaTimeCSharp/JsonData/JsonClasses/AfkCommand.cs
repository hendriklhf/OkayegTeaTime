using Newtonsoft.Json;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class AfkCommand
    {
        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("alias")]
        public List<string> Alias { get; set; }

        [JsonProperty("parameter")]
        public List<string> Parameter { get; set; }

        [JsonProperty("description")]
        public List<string> Description { get; set; }
    }
}
