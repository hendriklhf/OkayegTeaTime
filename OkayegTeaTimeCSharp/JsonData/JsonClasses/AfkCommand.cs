using Newtonsoft.Json;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class AfkCommand
    {
        [JsonProperty("CommandName")]
        public string CommandName { get; set; }

        [JsonProperty("ComingBack")]
        public string ComingBack { get; set; }

        [JsonProperty("GoingAway")]
        public string GoingAway { get; set; }

        [JsonProperty("Resuming")]
        public string Resuming { get; set; }

        [JsonProperty("Alias")]
        public List<string> Alias { get; set; }

        [JsonProperty("Parameter")]
        public List<string> Parameter { get; set; }

        [JsonProperty("Description")]
        public List<string> Description { get; set; }
    }
}