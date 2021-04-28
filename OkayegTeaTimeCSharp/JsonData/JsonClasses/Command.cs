using Newtonsoft.Json;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class Command
    {
        [JsonProperty("commandName")]
        public string CommandName { get; set; }

        [JsonProperty("alias")]
        public List<string> Alias { get; set; }

        [JsonProperty("parameter")]
        public List<string> Parameter { get; set; }

        [JsonProperty("description")]
        public List<string> Description { get; set; }
    }
}
