using Newtonsoft.Json;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class UserList
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Matches")]
        public List<string> Matches { get; set; }
    }
}
