using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Cache;
using OkayegTeaTime.Models.Bttv;
using OkayegTeaTime.Models.Ffz;
using OkayegTeaTime.Models.SevenTv;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class EmoteController
{
    public FfzEmote[] FfzGlobalEmotes
    {
        get
        {
            if (_ffzGlobalEmotes is not null)
            {
                return _ffzGlobalEmotes;
            }

            _ffzGlobalEmotes = GetFfzGlobalEmotes();
            return _ffzGlobalEmotes ?? Array.Empty<FfzEmote>();
        }
    }

    public BttvEmote[] BttvGlobalEmotes
    {
        get
        {
            if (_bttvGlobalEmotes is not null)
            {
                return _bttvGlobalEmotes;
            }

            _bttvGlobalEmotes = GetBttvGlobalEmotes();
            return _bttvGlobalEmotes ?? Array.Empty<BttvEmote>();
        }
    }

    public Emote[] SevenTvGlobalEmotes
    {
        get
        {
            if (_sevenTvGlobalEmotes is not null)
            {
                return _sevenTvGlobalEmotes;
            }

            _sevenTvGlobalEmotes = GetSevenTvGlobalEmotes();
            return _sevenTvGlobalEmotes ?? Array.Empty<Emote>();
        }
    }

    private readonly ChannelCache? _channelCache;

    private FfzEmote[]? _ffzGlobalEmotes;
    private BttvEmote[]? _bttvGlobalEmotes;
    private Emote[]? _sevenTvGlobalEmotes;

    private readonly Dictionary<long, FfzEmote[]> _ffzChannelsEmotes = new();
    private readonly Dictionary<long, BttvEmote[]> _bttvEmotes = new();
    private readonly Dictionary<long, Emote[]> _sevenTvChannelEmotes = new();

    public EmoteController(ChannelCache? channelCache = null)
    {
        _channelCache = channelCache;
    }

    public FfzEmote[] GetFfzEmotes(long channelId, bool loadFromCache = true)
    {
        if (loadFromCache && _ffzChannelsEmotes.TryGetValue(channelId, out FfzEmote[]? emotes))
        {
            return emotes;
        }

        FfzRequest? request = GetFfzRequest(channelId);
        emotes = request?.Set?.EmoteSet?.Emotes;
        if (emotes is not null && !_ffzChannelsEmotes.TryAdd(channelId, emotes))
        {
            _ffzChannelsEmotes[channelId] = emotes;
        }

        return emotes ?? Array.Empty<FfzEmote>();
    }

    public BttvEmote[] GetBttvEmotes(long channelId, bool loadFromCache = true)
    {
        if (loadFromCache && _bttvEmotes.TryGetValue(channelId, out BttvEmote[]? emotes))
        {
            return emotes;
        }

        BttvRequest? request = GetBttvRequest(channelId);
        emotes = request?.ChannelEmotes?.Concat(request.SharedEmotes).ToArray();
        if (emotes is not null && !_bttvEmotes.TryAdd(channelId, emotes))
        {
            _bttvEmotes[channelId] = emotes;
        }

        return emotes ?? Array.Empty<BttvEmote>();
    }

    public Emote[] GetSevenTvEmotes(long channelId, bool loadFromCache = true)
    {
        if (loadFromCache && _sevenTvChannelEmotes.TryGetValue(channelId, out Emote[]? emotes))
        {
            return emotes;
        }

        User? user = GetSevenTvUser(channelId);
        emotes = user?.EmoteSet?.Emotes;
        if (emotes is not null && !_sevenTvChannelEmotes.TryAdd(channelId, emotes))
        {
            _sevenTvChannelEmotes[channelId] = emotes;
        }

        return emotes ?? Array.Empty<Emote>();
    }

    private static User? GetSevenTvUser(long channelId)
    {
        try
        {
            HttpGet request = new($"https://7tv.io/v3/users/twitch/{channelId}");
            return string.IsNullOrWhiteSpace(request.Result) ? null : JsonSerializer.Deserialize<User>(request.Result);
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private static Emote[]? GetSevenTvGlobalEmotes()
    {
        try
        {
            HttpGet request = new("https://7tv.io/v3/emote-sets/global");
            EmoteSet? emoteSet = string.IsNullOrWhiteSpace(request.Result) ? null : JsonSerializer.Deserialize<EmoteSet>(request.Result);
            return emoteSet?.Emotes;
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private static BttvEmote[]? GetBttvGlobalEmotes()
    {
        try
        {
            HttpGet request = new("https://api.betterttv.net/3/cached/emotes/global");
            return request.Result is null ? null : JsonSerializer.Deserialize<BttvEmote[]>(request.Result);
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private static BttvRequest? GetBttvRequest(long channelId)
    {
        try
        {
            HttpGet request = new($"https://api.betterttv.net/3/cached/users/twitch/{channelId}");
            return request.Result is null ? null : JsonSerializer.Deserialize<BttvRequest>(request.Result);
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private FfzRequest? GetFfzRequest(long channelId)
    {
        try
        {
            string? channelName = _channelCache is null ? DbController.GetChannel(channelId)?.Name : _channelCache[channelId]?.Name;
            if (channelName is null)
            {
                return null;
            }

            HttpGet request = new($"https://api.frankerfacez.com/v1/room/{channelName}");
            if (request.Result is null)
            {
                return null;
            }

            JsonElement json = JsonSerializer.Deserialize<JsonElement>(request.Result);
            int setId = json.GetProperty("room").GetProperty("set").GetInt32();
            string result = request.Result.Replace($"\"{setId}\":", "\"mainSet\":");
            return JsonSerializer.Deserialize<FfzRequest>(result);
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private static FfzEmote[]? GetFfzGlobalEmotes()
    {
        try
        {
            HttpGet request = new("https://api.frankerfacez.com/v1/set/global");
            if (request.Result is null)
            {
                return null;
            }

            JsonElement json = JsonSerializer.Deserialize<JsonElement>(request.Result);
            int setId = json.GetProperty("default_sets")[0].GetInt32();
            string result = request.Result.Replace($"\"{setId}\":", "\"mainSet\":");
            json = JsonSerializer.Deserialize<JsonElement>(result);
            string firstSet = json.GetProperty("sets").GetProperty("mainSet").GetProperty("emoticons").GetRawText();
            string secondSet = json.GetProperty("sets").GetProperty("4330").GetProperty("emoticons").GetRawText();
            FfzEmote[] firstEmoteSet = JsonSerializer.Deserialize<FfzEmote[]>(firstSet) ?? Array.Empty<FfzEmote>();
            FfzEmote[] secondEmoteSet = JsonSerializer.Deserialize<FfzEmote[]>(secondSet) ?? Array.Empty<FfzEmote>();
            FfzEmote[] emotes = firstEmoteSet.Concat(secondEmoteSet).ToArray();
            return emotes;
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }
}
