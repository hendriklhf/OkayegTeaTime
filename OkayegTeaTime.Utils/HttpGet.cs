using System.Net.Http;
using System.Threading.Tasks;

namespace OkayegTeaTime.Utils;

public readonly struct HttpGet
{
    public string? Result { get; }

    public HttpGet(string url)
    {
        try
        {
            using HttpClient httpClient = new();
            Task<string> task = httpClient.GetStringAsync(url);
            task.Wait();
            Result = task.Result;
        }
        catch
        {
            Result = null;
        }
    }
}
