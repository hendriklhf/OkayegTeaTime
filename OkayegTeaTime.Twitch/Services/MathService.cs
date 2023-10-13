using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HLE.Collections;
using HLE.Strings;

namespace OkayegTeaTime.Twitch.Services;

public sealed class MathService : IEquatable<MathService>
{
    private readonly ConcurrentDictionary<string, string> _expressionResultCache = new();

    private const int _maximumCacheEntries = 1000;
    private const string _apiUrl = "https://api.mathjs.org/v4/?expr=";

    public async ValueTask<string> GetExpressionResultAsync(string expression)
    {
        expression = expression.TrimAll();
        if (_expressionResultCache.TryGetValue(expression, out string? result))
        {
            return result;
        }

        string url = $"{_apiUrl}{HttpUtility.UrlEncode(expression)}";
        using HttpClient httpClient = new();
        using HttpResponseMessage httpResponse = await httpClient.GetAsync(url);
        using HttpContentBytes httpContentBytes = await HttpContentBytes.CreateAsync(httpResponse);
        if (httpContentBytes.Length == 0)
        {
            throw new HttpResponseEmptyException();
        }

        result = Encoding.UTF8.GetString(httpContentBytes.AsSpan());

        ClearCacheIfThereAreTooManyEntries();
        _expressionResultCache.AddOrSet(expression, result);
        return result;
    }

    private void ClearCacheIfThereAreTooManyEntries()
    {
        if (_expressionResultCache.Count >= _maximumCacheEntries)
        {
            _expressionResultCache.Clear();
        }
    }

    public bool Equals(MathService? other)
    {
        return ReferenceEquals(this, other);
    }

    public override bool Equals(object? obj)
    {
        return obj is MathService other && Equals(other);
    }

    public override int GetHashCode()
    {
        return RuntimeHelpers.GetHashCode(this);
    }

    public static bool operator ==(MathService? left, MathService? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(MathService? left, MathService? right)
    {
        return !(left == right);
    }
}
