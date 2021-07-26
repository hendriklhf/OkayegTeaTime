using OkayegTeaTimeCSharp.Properties;
using System.IO;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.Data;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData;
using System.Text.Json;

namespace OkayegTeaTimeCSharp.JsonData
{
    public static class JsonController
    {
        public static Data BotData { get; private set; }

        public static CommandLists CommandLists { get; private set; }

        public static void LoadData()
        {
            BotData = JsonSerializer.Deserialize<Data>(File.ReadAllText(Resources.JsonPath));
            CommandLists = JsonSerializer.Deserialize<CommandLists>(File.ReadAllText(Resources.CommandsJsonPath));
        }
    }
}