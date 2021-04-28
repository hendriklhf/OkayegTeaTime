using Newtonsoft.Json;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class UserLists
    {
        [JsonProperty("SpecialUsers")]
        public List<string> SpecialUsers { get; set; }

        [JsonProperty("SecretUsers")]
        public List<string> SecretUsers { get; set; }
    }
}
