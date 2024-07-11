using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Threading.Tasks;
using OkayegTeaTime.Twitch.Helix.Models;
using OkayegTeaTime.Twitch.Helix.Models.Responses;

namespace OkayegTeaTime.Twitch.Helix;

public sealed partial class TwitchApi
{
    public async ValueTask<ImmutableArray<Emote>> GetGlobalEmotesAsync()
    {
        if (TryGetGlobalEmotesFromCache(out ImmutableArray<Emote> emotes))
        {
            return emotes;
        }

        using UrlBuilder urlBuilder = new(ApiBaseUrl, "chat/emotes/global");
        using HttpContentBytes response = await ExecuteRequestAsync(urlBuilder.ToString());
        GetResponse<Emote> getResponse = JsonSerializer.Deserialize(response.AsSpan(), HelixJsonSerializerContext.Default.GetResponseEmote);
        if (getResponse.Items.Length == 0)
        {
            throw new InvalidOperationException("An unknown error occurred. The response contained zero emotes.");
        }

        emotes = getResponse.Items;
        Cache?.AddGlobalEmotes(emotes);
        return emotes;
    }

    public async ValueTask<ImmutableArray<ChannelEmote>> GetChannelEmotesAsync(long channelId)
    {
        if (TryGetChannelEmotesFromCache(channelId, out ImmutableArray<ChannelEmote> emotes))
        {
            return emotes;
        }

        using UrlBuilder urlBuilder = new(ApiBaseUrl, "chat/emotes");
        urlBuilder.AppendParameter("broadcaster_id", channelId);
        using HttpContentBytes response = await ExecuteRequestAsync(urlBuilder.ToString());
        GetResponse<ChannelEmote> getResponse = JsonSerializer.Deserialize(response.AsSpan(), HelixJsonSerializerContext.Default.GetResponseChannelEmote);
        emotes = getResponse.Items;
        Cache?.AddChannelEmotes(channelId, emotes);
        return emotes;
    }

    private bool TryGetGlobalEmotesFromCache(out ImmutableArray<Emote> emotes)
    {
        emotes = [];
        return Cache?.TryGetGlobalEmotes(out emotes) == true;
    }

    private bool TryGetChannelEmotesFromCache(long channelId, out ImmutableArray<ChannelEmote> emotes)
    {
        emotes = [];
        return Cache?.TryGetChannelEmotes(channelId, out emotes) == true;
    }
}
