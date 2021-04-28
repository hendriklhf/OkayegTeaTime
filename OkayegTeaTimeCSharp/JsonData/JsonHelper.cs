using Newtonsoft.Json;
using OkayegTeaTimeCSharp.JsonData.JsonClasses;
using OkayegTeaTimeCSharp.Properties;
using System.IO;
using System.Text.Json;

namespace OkayegTeaTimeCSharp.JsonData
{
    public static class JsonHelper
    {
        public static string Serialize(Data data)
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };
            return System.Text.Json.JsonSerializer.Serialize(data, options);
        }

        public static Data Deserialize(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Data>(json);
        }

        public static Data JsonToObject()
        {
            return JsonConvert.DeserializeObject<Data>(Resources.Data.ToString());
        }

        public static void StringToJson(Data data)
        {
            File.WriteAllText(@"..\Resources\Data.json", Serialize(data));
        }
    }
}
