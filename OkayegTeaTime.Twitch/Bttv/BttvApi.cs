using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using HLE.Strings;
using OkayegTeaTime.Twitch.Bttv.Models;
using OkayegTeaTime.Twitch.Bttv.Models.Responses;

namespace OkayegTeaTime.Twitch.Bttv;

public sealed class BttvApi : IEquatable<BttvApi>
{
    public BttvApiCache? Cache { get; set; }

    private const string _apiBaseUrl = "https://api.betterttv.net/3";

    public BttvApi(CacheOptions? cacheOptions = null)
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

        using PooledStringBuilder urlBuilder = new(100);
        urlBuilder.Append(_apiBaseUrl, "/cached/users/twitch/");
        urlBuilder.Append(channelId);

        using HttpClient httpClient = new();
        using HttpResponseMessage httpResponse = await httpClient.GetAsync(urlBuilder.ToString());
        if (httpResponse.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
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

        GetUserResponse userResponse = JsonSerializer.Deserialize<GetUserResponse>(httpContentBytes.AsSpan());
        if (userResponse.ChannelEmotes.Length == 0 && userResponse.SharedEmotes.Length == 0)
        {
            return Array.Empty<Emote>();
        }

        emotes = new Emote[userResponse.ChannelEmotes.Length + userResponse.SharedEmotes.Length];
        userResponse.ChannelEmotes.CopyTo(emotes.AsSpan());
        userResponse.SharedEmotes.CopyTo(emotes.AsSpan(userResponse.ChannelEmotes.Length));
        Cache?.AddChannelEmotes(channelId, emotes);
        return emotes;
    }

    public async ValueTask<Emote[]> GetGlobalEmotesAsync()
    {
        if (TryGetGlobalEmotesFromCache(out Emote[]? emotes))
        {
            return emotes;
        }

        using PooledStringBuilder urlBuilder = new(100);
        urlBuilder.Append(_apiBaseUrl, "/cached/emotes/global");

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

        emotes = JsonSerializer.Deserialize<Emote[]>(httpContentBytes.AsSpan()) ?? throw new InvalidOperationException("The deserialization of the global emotes response failed and returned null.");
        Cache?.AddGlobalEmotes(emotes);
        return emotes;
    }

    private bool TryGetChannelEmotesFromCache(long channelId, [MaybeNullWhen(false)] out Emote[] emotes)
    {
        emotes = null;
        return Cache?.TryGetChannelEmotes(channelId, out emotes) == true;
    }

    private bool TryGetGlobalEmotesFromCache([MaybeNullWhen(false)] out Emote[] emotes)
    {
        emotes = null;
        return Cache?.TryGetGlobalEmotes(out emotes) == true;
    }

    public bool Equals(BttvApi? other)
    {
        return ReferenceEquals(this, other);
    }

    public override bool Equals(object? obj)
    {
        return obj is BttvApi other && Equals(other);
    }

    public override int GetHashCode()
    {
        return RuntimeHelpers.GetHashCode(this);
    }

    public static bool operator ==(BttvApi? left, BttvApi? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BttvApi? left, BttvApi? right)
    {
        return !(left == right);
    }
}
