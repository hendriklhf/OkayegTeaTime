using Newtonsoft.Json;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class Discord
    {
        [JsonProperty("UserList")]
        public List<UserList> UserList { get; set; }
    }
}