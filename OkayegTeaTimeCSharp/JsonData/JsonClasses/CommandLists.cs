using Newtonsoft.Json;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class CommandLists
    {
        [JsonProperty("Commands")]
        public List<Command> Commands { get; set; }

        [JsonProperty("AfkCommands")]
        public List<AfkCommand> AfkCommands { get; set; }
    }
}
