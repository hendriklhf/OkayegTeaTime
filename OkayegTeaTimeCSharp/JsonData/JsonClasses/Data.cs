using Newtonsoft.Json;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class Data
    {
        [JsonProperty("CommandLists")]
        public CommandLists CommandLists { get; set; }

        [JsonProperty("UserLists")]
        public UserLists UserLists { get; set; }

        [JsonProperty("EgData")]
        public EgData EgData { get; set; }

        [JsonProperty("Colors")]
        public List<string> Colors { get; set; }

        [JsonProperty("Discord")]
        public Discord Discord { get; set; }
    }
}