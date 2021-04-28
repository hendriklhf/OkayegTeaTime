using Newtonsoft.Json;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class UserLists
    {
        [JsonProperty("specialUsers")]
        public List<string> SpecialUsers { get; set; }

        [JsonProperty("secretUsers")]
        public List<string> SecretUsers { get; set; }
    }
}
