using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HLE.Http;
using HLE.Memory;
using HLE.Strings;
using OkayegTeaTime.Twitch.Api.SevenTv.Models;
using OkayegTeaTime.Twitch.Api.SevenTv.Models.Responses;

namespace OkayegTeaTime.Twitch.Api.SevenTv;

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

        using PoolBufferStringBuilder urlBuilder = new(_apiBaseUrl.Length + 30);
        urlBuilder.Append(_apiBaseUrl, "/emote-sets/global");

        using HttpClient httpClient = new();
        using HttpResponseMessage httpResponse = await httpClient.GetAsync(urlBuilder.ToString());
        int contentLength = httpResponse.GetContentLength();
        if (contentLength == 0)
        {
            throw new HttpResponseEmptyException();
        }

        using HttpContentBytes httpContentBytes = await httpResponse.GetContentBytesAsync(contentLength);
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestFailedException(httpResponse.StatusCode, httpContentBytes.Span);
        }

        GetGlobalEmotesResponse response = JsonSerializer.Deserialize<GetGlobalEmotesResponse>(httpContentBytes.Span);
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

        using PoolBufferStringBuilder urlBuilder = new(_apiBaseUrl.Length + 30);
        urlBuilder.Append(_apiBaseUrl, "/users/twitch/");
        urlBuilder.Append(channelId);

        using HttpClient httpClient = new();
        using HttpResponseMessage httpResponse = await httpClient.GetAsync(urlBuilder.ToString());
        if (httpResponse.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        int contentLength = httpResponse.GetContentLength();
        if (contentLength == 0)
        {
            throw new HttpResponseEmptyException();
        }

        using HttpContentBytes httpContentBytes = await httpResponse.GetContentBytesAsync(contentLength);
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestFailedException(httpResponse.StatusCode, httpContentBytes.Span);
        }

        GetChannelEmotesResponse response = JsonSerializer.Deserialize<GetChannelEmotesResponse>(httpContentBytes.Span);
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

    public bool Equals(SevenTvApi? other)
    {
        return ReferenceEquals(this, other);
    }

    public override bool Equals(object? obj)
    {
        return obj is SevenTvApi other && Equals(other);
    }

    public override int GetHashCode()
    {
        return MemoryHelper.GetRawDataPointer(this).GetHashCode();
    }

    public static bool operator ==(SevenTvApi? left, SevenTvApi? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(SevenTvApi? left, SevenTvApi? right)
    {
        return !(left == right);
    }
}
