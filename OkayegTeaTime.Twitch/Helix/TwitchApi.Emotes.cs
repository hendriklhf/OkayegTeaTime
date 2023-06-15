using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading.Tasks;
using HLE.Http;
using OkayegTeaTime.Twitch.Helix.Models;
using OkayegTeaTime.Twitch.Helix.Models.Responses;

namespace OkayegTeaTime.Twitch.Helix;

public sealed partial class TwitchApi
{
    public async ValueTask<Emote[]> GetGlobalEmotesAsync()
    {
        if (TryGetGlobalEmotesFromCache(out Emote[]? emotes))
        {
            return emotes;
        }

        using UrlBuilder urlBuilder = new(_apiBaseUrl, "chat/emotes/global");
        using HttpContentBytes response = await ExecuteRequestAsync(urlBuilder.ToString());
        GetResponse<Emote> getResponse = JsonSerializer.Deserialize<GetResponse<Emote>>(response.Span);
        if (getResponse.Items.Length == 0)
        {
            throw new InvalidOperationException("An unknown error occurred. The response contained zero emotes.");
        }

        emotes = getResponse.Items;
        Cache?.AddGlobalEmotes(emotes);
        return emotes;
    }

    public async ValueTask<ChannelEmote[]> GetChannelEmotesAsync(long channelId)
    {
        if (TryGetChannelEmotesFromCache(channelId, out ChannelEmote[]? emotes))
        {
            return emotes;
        }

        using UrlBuilder urlBuilder = new(_apiBaseUrl, "chat/emotes");
        urlBuilder.AppendParameter("broadcaster_id", channelId);
        using HttpContentBytes response = await ExecuteRequestAsync(urlBuilder.ToString());
        GetResponse<ChannelEmote> getResponse = JsonSerializer.Deserialize<GetResponse<ChannelEmote>>(response.Span);
        emotes = getResponse.Items;
        Cache?.AddChannelEmotes(channelId, emotes);
        return emotes;
    }

    private bool TryGetGlobalEmotesFromCache([MaybeNullWhen(false)] out Emote[] emotes)
    {
        emotes = null;
        return Cache?.TryGetGlobalEmotes(out emotes) == true;
    }

    private bool TryGetChannelEmotesFromCache(long channelId, [MaybeNullWhen(false)] out ChannelEmote[] emotes)
    {
        emotes = null;
        return Cache?.TryGetChannelEmotes(channelId, out emotes) == true;
    }
}
