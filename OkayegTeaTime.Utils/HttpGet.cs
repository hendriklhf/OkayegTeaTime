using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OkayegTeaTime.Utils;

public readonly struct HttpGet : IEquatable<HttpGet>
{
    public string? Result { get; } = null;

    public static HttpGet Empty => new();

    public HttpGet()
    {
    }

    private HttpGet(string? result)
    {
        Result = result;
    }

    public static HttpGet FromResult(string? result)
    {
        return new(result);
    }

    public static async ValueTask<HttpGet> GetStringAsync(string url)
    {
        try
        {
            using HttpClient httpClient = new();
            string result = await httpClient.GetStringAsync(url);
            return new(result);
        }
        catch
        {
            return Empty;
        }
    }

    public bool Equals(HttpGet other)
    {
        return Result == other.Result;
    }

    public override bool Equals(object? obj)
    {
        return obj is HttpGet other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Result?.GetHashCode() ?? 0;
    }

    public static bool operator ==(HttpGet left, HttpGet right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(HttpGet left, HttpGet right)
    {
        return !(left == right);
    }
}
