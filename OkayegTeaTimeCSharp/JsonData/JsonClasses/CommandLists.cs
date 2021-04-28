using Newtonsoft.Json;
using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses
{
    public class CommandLists
    {
        [JsonProperty("commands")]
        public List<Command> Commands { get; set; }

        [JsonProperty("afkCommands")]
        public List<AfkCommand> AfkCommands { get; set; }
    }
}
