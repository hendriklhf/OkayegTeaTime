using OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.Data;
using OkayegTeaTimeCSharp.Properties;
using System.IO;
using System.Text.Json;

namespace OkayegTeaTimeCSharp.JsonData
{
    public class JsonController
    {
        public BotData BotData => _botData;

        public CommandLists CommandLists => _commandLists;

        private static BotData _botData;
        private static CommandLists _commandLists;

        public void LoadData()
        {
            _botData = JsonSerializer.Deserialize<BotData>(File.ReadAllText(Resources.JsonPath));
            _commandLists = JsonSerializer.Deserialize<CommandLists>(File.ReadAllText(Resources.CommandsJsonPath));
        }
    }
}