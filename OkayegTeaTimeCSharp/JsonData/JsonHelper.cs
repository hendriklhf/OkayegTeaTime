using OkayegTeaTimeCSharp.JsonData.JsonClasses;
using OkayegTeaTimeCSharp.Properties;
using System.IO;
using NewtonJson = Newtonsoft.Json;
using SystemJson = System.Text.Json;

namespace OkayegTeaTimeCSharp.JsonData
{
    public static class JsonHelper
    {
        public static Data BotData { get; private set; }

        public static void SetData()
        {
            BotData = JsonToObject();
        }

        public static void UpdateJson()
        {
            ObjectToJson(BotData);
            SetData();
        }

        private static Data JsonToObject()
        {
            return NewtonJson::JsonConvert.DeserializeObject<Data>(JsonToString());
        }

        private static string JsonToString()
        {
            return File.ReadAllText(Resources.JsonPath);
        }

        private static void ObjectToJson(Data data)
        {
            File.WriteAllText(Resources.JsonPath, ObjectToString(data));
        }

        private static string ObjectToString(Data data)
        {
            SystemJson::JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };
            return SystemJson::JsonSerializer.Serialize(data, options);
        }

        private static Data StringToObject(string json)
        {
            return SystemJson::JsonSerializer.Deserialize<Data>(json);
        }
    }
}