using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HLE.Collections;

namespace OkayegTeaTime.Utils;

public readonly struct HttpPost
{
    public string? Result { get; }

    public HttpPost(string url, IEnumerable<(string, string)> content)
    {
        try
        {
            using HttpClient httpClient = new();
            using FormUrlEncodedContent encodedContent = new(content.ToDictionary<IEnumerable<(string, string)>, string, string>());
            Task<HttpResponseMessage> postTask = httpClient.PostAsync(url, encodedContent);
            postTask.Wait();
            HttpResponseMessage result = postTask.Result;
            Task<string> readTask = result.Content.ReadAsStringAsync();
            readTask.Wait();
            Result = readTask.Result;
        }
        catch
        {
            Result = null;
        }
    }
}
