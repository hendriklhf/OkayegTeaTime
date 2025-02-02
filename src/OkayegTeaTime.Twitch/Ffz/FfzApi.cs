using System;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using HLE.Text;
using OkayegTeaTime.Twitch.Ffz.Models;

namespace OkayegTeaTime.Twitch.Ffz;

public sealed class FfzApi : IEquatable<FfzApi>
{
    public FfzApiCache? Cache { get; set; }

    private const string ApiBaseUrl = "https://api.frankerfacez.com/v1";

    public FfzApi(CacheOptions? cacheOptions = null)
    {
        if (cacheOptions is not null)
        {
            Cache = new(cacheOptions);
        }
    }

    public async ValueTask<ImmutableArray<Emote>> GetChannelEmotesAsync(long channelId)
    {
        if (TryGetChannelEmotesFromCache(channelId, out ImmutableArray<Emote> emotes))
        {
            return emotes;
        }

        using PooledStringBuilder urlBuilder = new(ApiBaseUrl.Length + 30);
        urlBuilder.Append(ApiBaseUrl, "/room/id/");
        urlBuilder.Append(channelId);

        using HttpClient httpClient = new();
        using HttpResponseMessage httpResponse = await httpClient.GetAsync(urlBuilder.ToString());
        if (httpResponse.StatusCode == HttpStatusCode.NotFound)
        {
            return [];
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

        Room room = JsonSerializer.Deserialize(httpContentBytes.AsSpan(), FfzJsonSerializerContext.Default.GetRoomResponse).Room;
        if (room == Room.Empty)
        {
            return [];
        }

        emotes = DeserializeResponse(httpContentBytes.AsSpan());
        Cache?.AddChannelEmotes(channelId, room.TwitchUsername, emotes);
        return emotes;
    }

    // ReSharper disable once InconsistentNaming
    public ValueTask<ImmutableArray<Emote>> GetChannelEmotesAsync(string channelName) => GetChannelEmotesAsync(channelName.AsMemory());

    public async ValueTask<ImmutableArray<Emote>> GetChannelEmotesAsync(ReadOnlyMemory<char> channelName)
    {
        if (TryGetChannelEmotesFromCache(channelName.Span, out ImmutableArray<Emote> emotes))
        {
            return emotes;
        }

        using PooledStringBuilder urlBuilder = new(ApiBaseUrl.Length + 30);
        urlBuilder.Append($"{ApiBaseUrl}/room/{channelName.Span}");

        using HttpClient httpClient = new();
        using HttpResponseMessage httpResponse = await httpClient.GetAsync(urlBuilder.ToString());
        if (httpResponse.StatusCode == HttpStatusCode.NotFound)
        {
            return [];
        }

        using HttpContentBytes httpContentBytes = await HttpContentBytes.CreateAsync(httpResponse);
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestFailedException(httpResponse.StatusCode, httpContentBytes.AsSpan());
        }

        if (httpContentBytes.Length == 0)
        {
            throw new HttpResponseEmptyException();
        }

        Room room = JsonSerializer.Deserialize(httpContentBytes.AsSpan(), FfzJsonSerializerContext.Default.GetRoomResponse).Room;
        if (room == Room.Empty)
        {
            return [];
        }

        emotes = DeserializeResponse(httpContentBytes.AsSpan());
        Cache?.AddChannelEmotes(room.TwitchId, channelName.Span, emotes);
        return emotes;
    }

    public async ValueTask<ImmutableArray<Emote>> GetGlobalEmotesAsync()
    {
        if (TryGetGlobalEmotesFromCache(out ImmutableArray<Emote> emotes))
        {
            return emotes;
        }

        using PooledStringBuilder urlBuilder = new(ApiBaseUrl.Length + 30);
        urlBuilder.Append(ApiBaseUrl, "/set/global");

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

        emotes = JsonSerializer.Deserialize(httpContentBytes.AsSpan(), FfzJsonSerializerContext.Default.GetGlobalEmotesResponse).Sets.GlobalSet.Emotes;
        if (emotes.Length == 0)
        {
            return emotes;
        }

        Cache?.AddGlobalEmotes(emotes);
        return emotes;
    }

    private bool TryGetGlobalEmotesFromCache(out ImmutableArray<Emote> emotes)
    {
        emotes = [];
        return Cache?.TryGetGlobalEmotes(out emotes) == true;
    }

    private bool TryGetChannelEmotesFromCache(long channelId, out ImmutableArray<Emote> emotes)
    {
        emotes = [];
        return Cache?.TryGetChannelEmotes(channelId, out emotes) == true;
    }

    private bool TryGetChannelEmotesFromCache(ReadOnlySpan<char> channel, out ImmutableArray<Emote> emotes)
    {
        emotes = [];
        return Cache?.TryGetChannelEmotes(channel, out emotes) == true;
    }

    private static ImmutableArray<Emote> DeserializeResponse(ReadOnlySpan<byte> response)
    {
        ResponseDeserializer deserializer = new(response);
        return deserializer.Deserialize();
    }

    public bool Equals(FfzApi? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj) => obj is FfzApi other && Equals(other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(FfzApi? left, FfzApi? right) => Equals(left, right);

    public static bool operator !=(FfzApi? left, FfzApi? right) => !(left == right);
}
