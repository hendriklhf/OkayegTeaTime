using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HLE.Memory;
using HLE.Text;

namespace OkayegTeaTime.Twitch.Services;

public sealed class MathService : IDisposable, IEquatable<MathService>
{
    private readonly HttpClient _httpClient = new();

    private const string ApiUrl = "https://api.mathjs.org/v4/?expr=";

    public void Dispose() => _httpClient.Dispose();

    public async Task GetExpressionResultAsync(ReadOnlyMemory<char> expression, PooledStringBuilder destination)
    {
        string url = ApiUrl + UrlEncodeExpression(expression.Span);
        using HttpResponseMessage httpResponse = await _httpClient.GetAsync(url);
        using HttpContentBytes httpContentBytes = await HttpContentBytes.CreateAsync(httpResponse);
        if (httpContentBytes.Length == 0)
        {
            throw new HttpResponseEmptyException();
        }

        int charCount = Encoding.UTF8.GetMaxCharCount(httpContentBytes.Length);
        destination.EnsureCapacity(destination.Length + charCount);
        charCount = Encoding.UTF8.GetChars(httpContentBytes.AsSpan(), destination.FreeBufferSpan);
        destination.Advance(charCount);
    }

    private static string UrlEncodeExpression(ReadOnlySpan<char> expression)
    {
        byte[] bytes = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(expression.Length));
        int byteCount = Encoding.UTF8.GetBytes(expression, bytes.AsSpan());
        string url = HttpUtility.UrlEncode(bytes, 0, byteCount);
        ArrayPool<byte>.Shared.Return(bytes);
        return url;
    }

    public bool Equals(MathService? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj) => obj is MathService other && Equals(other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(MathService? left, MathService? right) => Equals(left, right);

    public static bool operator !=(MathService? left, MathService? right) => !(left == right);
}
