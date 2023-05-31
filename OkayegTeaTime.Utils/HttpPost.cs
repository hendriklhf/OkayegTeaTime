using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HLE.Collections;

namespace OkayegTeaTime.Utils;

public readonly struct HttpPost
{
    public string? Result { get; } = null;

    public static HttpPost Empty => new();

    public HttpPost()
    {
    }

    private HttpPost(string? result)
    {
        Result = result;
    }

    public static HttpPost FromResult(string? result)
    {
        return new(result);
    }

    public static async ValueTask<HttpPost> GetStringAsync(string url, IEnumerable<(string, string)> content)
    {
        try
        {
            using HttpClient httpClient = new();
            using FormUrlEncodedContent encodedContent = new(content.ToDictionary());
            using HttpResponseMessage response = await httpClient.PostAsync(url, encodedContent);
            string result = await response.Content.ReadAsStringAsync();
            return new(result);
        }
        catch
        {
            return Empty;
        }
    }
}
