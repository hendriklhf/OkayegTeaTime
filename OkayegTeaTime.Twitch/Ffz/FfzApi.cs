using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HLE.Http;
using HLE.Memory;
using HLE.Strings;
using OkayegTeaTime.Twitch.Ffz.Models;
using OkayegTeaTime.Twitch.Ffz.Models.Responses;

namespace OkayegTeaTime.Twitch.Ffz;

public sealed class FfzApi : IEquatable<FfzApi>
{
    public FfzApiCache? Cache { get; set; }

    private const string _apiBaseUrl = "https://api.frankerfacez.com/v1";

    public FfzApi(CacheOptions? cacheOptions = null)
    {
        if (cacheOptions is not null)
        {
            Cache = new(cacheOptions);
        }
    }

    public async ValueTask<Emote[]?> GetChannelEmotesAsync(long channelId)
    {
        if (TryGetChannelEmotesFromCache(channelId, out Emote[]? emotes))
        {
            return emotes;
        }

        using PoolBufferStringBuilder urlBuilder = new(_apiBaseUrl.Length + 30);
        urlBuilder.Append(_apiBaseUrl, "/room/id/");
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

        Room room = JsonSerializer.Deserialize<GetRoomResponse>(httpContentBytes.Span).Room;
        if (room == Room.Empty)
        {
            return null;
        }

        emotes = DeserializeResponse(httpContentBytes.Span);
        Cache?.AddChannelEmotes(channelId, room.TwitchUsername, emotes);
        return emotes;
    }

    public async ValueTask<Emote[]?> GetChannelEmotesAsync(string channelName)
    {
        return await GetChannelEmotesAsync(channelName.AsMemory());
    }

    public async ValueTask<Emote[]?> GetChannelEmotesAsync(ReadOnlyMemory<char> channelName)
    {
        if (TryGetChannelEmotesFromCache(channelName.Span, out Emote[]? emotes))
        {
            return emotes;
        }

        using PoolBufferStringBuilder urlBuilder = new(_apiBaseUrl.Length + 30);
        urlBuilder.Append(_apiBaseUrl, "/room/", channelName.Span);

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

        Room room = JsonSerializer.Deserialize<GetRoomResponse>(httpContentBytes.Span).Room;
        if (room == Room.Empty)
        {
            return null;
        }

        emotes = DeserializeResponse(httpContentBytes.Span);
        Cache?.AddChannelEmotes(room.TwitchId, channelName.Span, emotes);
        return emotes;
    }

    public async ValueTask<Emote[]> GetGlobalEmotesAsync()
    {
        if (TryGetGlobalEmotesFromCache(out Emote[]? emotes))
        {
            return emotes;
        }

        using PoolBufferStringBuilder urlBuilder = new(_apiBaseUrl.Length + 30);
        urlBuilder.Append(_apiBaseUrl, "/set/global");

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

        emotes = JsonSerializer.Deserialize<GetGlobalEmotesResponse>(httpContentBytes.Span).Sets.GlobalSet.Emotes;
        if (emotes.Length == 0)
        {
            return emotes;
        }

        Cache?.AddGlobalEmotes(emotes);
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

    private bool TryGetChannelEmotesFromCache(ReadOnlySpan<char> channel, [MaybeNullWhen(false)] out Emote[] emotes)
    {
        emotes = null;
        return Cache?.TryGetChannelEmotes(channel, out emotes) == true;
    }

    private static Emote[] DeserializeResponse(ReadOnlySpan<byte> response)
    {
        ResponseDeserializer deserializer = new(response);
        return deserializer.Deserialize();
    }

    public bool Equals(FfzApi? other)
    {
        return ReferenceEquals(this, other);
    }

    public override bool Equals(object? obj)
    {
        return obj is FfzApi other && Equals(other);
    }

    public override int GetHashCode()
    {
        return MemoryHelper.GetRawDataPointer(this).GetHashCode();
    }

    public static bool operator ==(FfzApi? left, FfzApi? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(FfzApi? left, FfzApi? right)
    {
        return !(left == right);
    }
}
