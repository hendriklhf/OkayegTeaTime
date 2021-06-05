using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OkayegTeaTimeCSharp.HttpRequests
{
    public class HttpGet
    {
        public string URL { get; }

        public string Result { get; }

        public JsonElement Data { get; }

        public bool ValidJsonData { get; }

        private readonly HttpClient _httpClient = new();

        public HttpGet(string url)
        {
            URL = url;
            Result = GetRequest().Result;
            try
            {
                Data = JsonSerializer.Deserialize<JsonElement>(Result);
                ValidJsonData = true;
            }
            catch (JsonException)
            {
                ValidJsonData = false;
            }
        }

        private async Task<string> GetRequest()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(URL);
            return await response.Content.ReadAsStringAsync();
        }
    }
}