using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OkayegTeaTimeCSharp.HttpRequests
{
    public class HttpGet
    {
        public string URL { get; }

        public JsonElement Data { get; private set; }

        private readonly HttpClient _httpClient = new();

        public HttpGet(string url)
        {
            URL = url;
            Data = JsonSerializer.Deserialize<JsonElement>(GetRequest().Result);
        }

        private async Task<string> GetRequest()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(URL);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
