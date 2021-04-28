using System.Text.Json;
using OkayegTeaTimeCSharp.JsonData.JsonClasses;

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
            return JsonSerializer.Serialize(data, options);
        }

        public static Data Deserialize(string json)
        {
            return JsonSerializer.Deserialize<Data>(json);
        }
    }
}
