using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OkayegTeaTimeCSharp.HttpRequests
{
    public class HttpPost
    {
        public string URL { get; }

        public HttpContent HeaderContent { get; }

        public JsonElement Data { get; }

        public bool ValdiJsonData { get; }

        private readonly HttpClient _httpClient = new();

        public HttpPost(string url, List<KeyValuePair<string, string>> headers)
        {
            URL = url;
            HeaderContent = new FormUrlEncodedContent(headers);
            try
            {
                Data = JsonSerializer.Deserialize<JsonElement>(PostRequest().Result);
                ValdiJsonData = true;
            }
            catch (JsonException)
            {
                ValdiJsonData = false;
            }
        }

        private async Task<string> PostRequest()
        {
            HttpResponseMessage response = await _httpClient.PostAsync(URL, HeaderContent);
            return await response.Content.ReadAsStringAsync();
        }
    }
}