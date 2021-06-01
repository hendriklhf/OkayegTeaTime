using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OkayegTeaTimeCSharp.HttpRequests
{
    public class HttpPost
    {
        public string URL { get; }

        public JsonElement Data { get; private set; }

        private readonly HttpClient _httpClient = new();

        public HttpPost(string url)
        {
            URL = url;
            Data = JsonSerializer.Deserialize<JsonElement>(PostRequest().Result);
        }

        private async Task<string> PostRequest()
        {
            return "";
        }
    }
}
