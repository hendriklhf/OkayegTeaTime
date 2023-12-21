using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using OkayegTeaTime.Twitch.Helix.Models;

namespace OkayegTeaTime.Twitch.Helix;

public sealed partial class TwitchApi : IEquatable<TwitchApi>, IDisposable
{
    public TwitchApiCache? Cache { get; set; }

    private readonly string _clientId;
    private AccessToken _accessToken = AccessToken.Empty;
    private readonly FormUrlEncodedContent _accessTokenRequestContent;

    private const string ApiBaseUrl = "https://api.twitch.tv/helix";

    public TwitchApi(string clientId, string clientSecret, CacheOptions? cacheOptions = null)
    {
        _clientId = clientId;
        if (cacheOptions is not null)
        {
            Cache = new(cacheOptions);
        }

        _accessTokenRequestContent = new(new[]
        {
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });
    }

    public void Dispose() => _accessTokenRequestContent.Dispose();

    private async ValueTask<HttpClient> CreateHttpClientAsync()
    {
        await EnsureValidAccessTokenAsync();
        HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");
        httpClient.DefaultRequestHeaders.Add("Client-Id", _clientId);
        return httpClient;
    }

    public async ValueTask<AccessToken> GetAccessTokenAsync()
    {
        using HttpClient httpClient = new();
        using HttpResponseMessage httpResponse = await httpClient.PostAsync("https://id.twitch.tv/oauth2/token", _accessTokenRequestContent);
        using HttpContentBytes httpContentBytes = await HttpContentBytes.CreateAsync(httpResponse);
        if (httpContentBytes.Length == 0)
        {
            throw new HttpResponseEmptyException();
        }

        return JsonSerializer.Deserialize(httpContentBytes.AsSpan(), HelixJsonSerializerContext.Default.AccessToken);
    }

    private async ValueTask EnsureValidAccessTokenAsync()
    {
        if (_accessToken != AccessToken.Empty && _accessToken.IsValid)
        {
            return;
        }

        _accessToken = await GetAccessTokenAsync();
    }

    private async ValueTask<HttpContentBytes> ExecuteRequestAsync(string url)
    {
        using HttpClient httpClient = await CreateHttpClientAsync();
        using HttpResponseMessage httpResponse = await httpClient.GetAsync(url);
        HttpContentBytes httpContentBytes = await HttpContentBytes.CreateAsync(httpResponse);
        if (httpContentBytes.Length == 0)
        {
            throw new HttpResponseEmptyException();
        }

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestFailedException(httpResponse.StatusCode, httpContentBytes.AsSpan());
        }

        return httpContentBytes;
    }

    public bool Equals(TwitchApi? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj) => obj is TwitchApi other && Equals(other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(TwitchApi? left, TwitchApi? right) => Equals(left, right);

    public static bool operator !=(TwitchApi? left, TwitchApi? right) => !(left == right);
}
