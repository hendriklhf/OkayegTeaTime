using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OkayegTeaTimeCSharp.HttpRequests
{
    public class HttpPost
    {
        public string URL { get; }

        public string Result { get; }

        public HttpContent HeaderContent { get; }

        public JsonElement Data { get; }

        public bool ValidJsonData { get; } = true;

        private readonly HttpClient _httpClient = new();

        public HttpPost(string url, List<KeyValuePair<string, string>> headers)
        {
            URL = url;
            HeaderContent = new FormUrlEncodedContent(headers);
            Result = PostRequest().Result;
            try
            {
                Data = JsonSerializer.Deserialize<JsonElement>(Result);
            }
            catch (JsonException)
            {
                ValidJsonData = false;
            }
        }

        private async Task<string> PostRequest()
        {
            HttpResponseMessage response = await _httpClient.PostAsync(URL, HeaderContent);
            return await response.Content.ReadAsStringAsync();
        }
    }
}