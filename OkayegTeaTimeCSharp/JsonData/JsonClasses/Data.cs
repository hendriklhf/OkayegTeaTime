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

        [JsonProperty("Channels")]
        public List<Channel> Channels { get; set; }

        [JsonProperty("Discord")]
        public Discord Discord { get; set; }
    }
}