using Newtonsoft.Json;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class Discord
    {
        [JsonProperty("userList")]
        public List<UserList> UserList { get; set; }
    }
}
