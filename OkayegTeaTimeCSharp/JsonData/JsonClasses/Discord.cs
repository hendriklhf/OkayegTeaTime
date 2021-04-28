using Newtonsoft.Json;
using System.Collections.Generic;

public class Discord
{
    [JsonProperty("userList")]
    public List<UserList> UserList { get; set; }
}
