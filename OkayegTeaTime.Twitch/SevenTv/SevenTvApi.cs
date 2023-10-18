using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using HLE.Strings;
using OkayegTeaTime.Twitch.SevenTv.Models;
using OkayegTeaTime.Twitch.SevenTv.Models.Responses;

namespace OkayegTeaTime.Twitch.SevenTv;

public sealed class SevenTvApi : IEquatable<SevenTvApi>
{
    private const string _apiBaseUrl = "https://7tv.io/v3";

    public SevenTvApiCache? Cache { get; set; }

    public SevenTvApi(CacheOptions? options = null)
    {
        if (options is not null)
        {
            Cache = new(options);
        }
    }

    public async ValueTask<Emote[]> GetGlobalEmotesAsync()
    {
        if (TryGetGlobalEmotesFromCache(out Emote[]? emotes))
        {
            return emotes;
        }

        using PooledStringBuilder urlBuilder = new(_apiBaseUrl.Length + 30);
        urlBuilder.Append(_apiBaseUrl, "/emote-sets/global");

        using HttpClient httpClient = new();
        using HttpResponseMessage httpResponse = await httpClient.GetAsync(urlBuilder.ToString());
        using HttpContentBytes httpContentBytes = await HttpContentBytes.CreateAsync(httpResponse);
        if (httpContentBytes.Length == 0)
        {
            throw new HttpResponseEmptyException();
        }

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestFailedException(httpResponse.StatusCode, httpContentBytes.AsSpan());
        }

        GetGlobalEmotesResponse response = JsonSerializer.Deserialize<GetGlobalEmotesResponse>(httpContentBytes.AsSpan());
        emotes = response.Emotes;
        Cache?.AddGlobalEmotes(emotes);
        return emotes;
    }

    public async ValueTask<Emote[]?> GetChannelEmotesAsync(long channelId)
    {
        if (TryGetChannelEmotesFromCache(channelId, out Emote[]? emotes))
        {
            return emotes;
        }

        using PooledStringBuilder urlBuilder = new(_apiBaseUrl.Length + 30);
        urlBuilder.Append(_apiBaseUrl, "/users/twitch/");
        urlBuilder.Append(channelId);

        using HttpClient httpClient = new();
        using HttpResponseMessage httpResponse = await httpClient.GetAsync(urlBuilder.ToString());
        if (httpResponse.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        using HttpContentBytes httpContentBytes = await HttpContentBytes.CreateAsync(httpResponse);
        if (httpContentBytes.Length == 0)
        {
            throw new HttpResponseEmptyException();
        }

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestFailedException(httpResponse.StatusCode, httpContentBytes.AsSpan());
        }

        GetChannelEmotesResponse response = JsonSerializer.Deserialize<GetChannelEmotesResponse>(httpContentBytes.AsSpan());
        emotes = response.EmoteSet.Emotes;
        Cache?.AddChannelEmotes(channelId, emotes);
        return emotes;
    }

    private bool TryGetGlobalEmotesFromCache([MaybeNullWhen(false)] out Emote[] emotes)
    {
        emotes = null;
        return Cache?.TryGetGlobalEmotes(out emotes) == true;
    }

    private bool TryGetChannelEmotesFromCache(long channelId, [MaybeNullWhen(false)] out Emote[] emotes)
    {
        emotes = null;
        return Cache?.TryGetChannelEmotes(channelId, out emotes) == true;
    }

    public bool Equals(SevenTvApi? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj) => obj is SevenTvApi other && Equals(other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(SevenTvApi? left, SevenTvApi? right) => Equals(left, right);

    public static bool operator !=(SevenTvApi? left, SevenTvApi? right) => !(left == right);
}
