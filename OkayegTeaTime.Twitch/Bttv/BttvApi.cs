using System;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using HLE.Strings;
using OkayegTeaTime.Twitch.Bttv.Models;
using OkayegTeaTime.Twitch.Bttv.Models.Responses;

namespace OkayegTeaTime.Twitch.Bttv;

public sealed class BttvApi : IEquatable<BttvApi>
{
    public BttvApiCache? Cache { get; set; }

    private const string ApiBaseUrl = "https://api.betterttv.net/3";

    public BttvApi(CacheOptions? cacheOptions = null)
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

        using PooledStringBuilder urlBuilder = new(100);
        urlBuilder.Append(ApiBaseUrl, "/cached/users/twitch/");
        urlBuilder.Append(channelId);

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

        GetUserResponse userResponse = JsonSerializer.Deserialize(httpContentBytes.AsSpan(), BttvJsonSerializerContext.Default.GetUserResponse);
        if (userResponse.ChannelEmotes.Length == 0 && userResponse.SharedEmotes.Length == 0)
        {
            return [];
        }

        Emote[] emoteArray = new Emote[userResponse.ChannelEmotes.Length + userResponse.SharedEmotes.Length];
        userResponse.ChannelEmotes.CopyTo(emoteArray.AsSpan());
        userResponse.SharedEmotes.CopyTo(emoteArray.AsSpan(userResponse.ChannelEmotes.Length));
        emotes = ImmutableCollectionsMarshal.AsImmutableArray(emoteArray);
        Cache?.AddChannelEmotes(channelId, emotes);
        return emotes;
    }

    public async ValueTask<ImmutableArray<Emote>> GetGlobalEmotesAsync()
    {
        if (TryGetGlobalEmotesFromCache(out ImmutableArray<Emote> emotes))
        {
            return emotes;
        }

        using PooledStringBuilder urlBuilder = new(100);
        urlBuilder.Append(ApiBaseUrl, "/cached/emotes/global");

        using HttpClient httpClient = new();
        using HttpResponseMessage httpResponse = await httpClient.GetAsync(urlBuilder.ToString());
        using HttpContentBytes httpContentBytes = await HttpContentBytes.CreateAsync(httpResponse);
        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new HttpRequestFailedException(httpResponse.StatusCode, httpContentBytes.AsSpan());
        }

        if (httpContentBytes.Length == 0)
        {
            throw new HttpResponseEmptyException();
        }

        emotes = JsonSerializer.Deserialize(httpContentBytes.AsSpan(), BttvJsonSerializerContext.Default.ImmutableArrayEmote);
        Cache?.AddGlobalEmotes(emotes);
        return emotes;
    }

    private bool TryGetChannelEmotesFromCache(long channelId, out ImmutableArray<Emote> emotes)
    {
        emotes = [];
        return Cache?.TryGetChannelEmotes(channelId, out emotes) == true;
    }

    private bool TryGetGlobalEmotesFromCache(out ImmutableArray<Emote> emotes)
    {
        emotes = [];
        return Cache?.TryGetGlobalEmotes(out emotes) == true;
    }

    public bool Equals(BttvApi? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj) => obj is BttvApi other && Equals(other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(BttvApi? left, BttvApi? right) => Equals(left, right);

    public static bool operator !=(BttvApi? left, BttvApi? right) => !(left == right);
}
