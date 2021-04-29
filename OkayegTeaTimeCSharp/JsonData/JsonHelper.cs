using OkayegTeaTimeCSharp.JsonData.JsonClasses;
using OkayegTeaTimeCSharp.Properties;
using System.IO;
using NewtonJson = Newtonsoft.Json;
using SystemJson = System.Text.Json;

namespace OkayegTeaTimeCSharp.JsonData
{
    public static class JsonHelper
    {
        public static string ObjectToString(Data data)
        {
            SystemJson::JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };
            return SystemJson::JsonSerializer.Serialize(data, options);
        }

        public static Data StringToObject(string json)
        {
            return SystemJson::JsonSerializer.Deserialize<Data>(json);
        }

        public static Data JsonToObject()
        {
            return NewtonJson::JsonConvert.DeserializeObject<Data>(JsonToString());
        }

        public static void ObjectToJson(Data data)
        {
            File.WriteAllText(Resources.JsonPath, ObjectToString(data));
        }

        public static string JsonToString()
        {
            return File.ReadAllText(Resources.JsonPath);
        }
    }
}
