using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using HLE.Collections;
using HLE.Memory;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using Ffz = OkayegTeaTime.Models.Ffz;
using Bttv = OkayegTeaTime.Models.Bttv;
using SevenTv = OkayegTeaTime.Models.SevenTv;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class EmoteController
{
    public ReadOnlySpan<Ffz.Emote> FfzGlobalEmotes
    {
        get
        {
            if (_ffzGlobalEmotes != EmoteEntry<Ffz.Emote>.Empty && _ffzGlobalEmotes.IsValid(_globalEmoteCacheTime))
            {
                return _ffzGlobalEmotes.Emotes;
            }

            _ffzGlobalEmotes = new(GetFfzGlobalEmotes());
            return _ffzGlobalEmotes.Emotes;
        }
    }

    public ReadOnlySpan<Bttv.Emote> BttvGlobalEmotes
    {
        get
        {
            if (_bttvGlobalEmotes != EmoteEntry<Bttv.Emote>.Empty && _bttvGlobalEmotes.IsValid(_globalEmoteCacheTime))
            {
                return _bttvGlobalEmotes.Emotes;
            }

            _bttvGlobalEmotes = new(GetBttvGlobalEmotes());
            return _bttvGlobalEmotes.Emotes;
        }
    }

    public ReadOnlySpan<SevenTv.Emote> SevenTvGlobalEmotes
    {
        get
        {
            if (_sevenTvGlobalEmotes != EmoteEntry<SevenTv.Emote>.Empty && _sevenTvGlobalEmotes.IsValid(_globalEmoteCacheTime))
            {
                return _sevenTvGlobalEmotes.Emotes;
            }

            _sevenTvGlobalEmotes = new(GetSevenTvGlobalEmotes());
            return _sevenTvGlobalEmotes.Emotes;
        }
    }

    private EmoteEntry<Ffz.Emote> _ffzGlobalEmotes = EmoteEntry<Ffz.Emote>.Empty;
    private EmoteEntry<Bttv.Emote> _bttvGlobalEmotes = EmoteEntry<Bttv.Emote>.Empty;
    private EmoteEntry<SevenTv.Emote> _sevenTvGlobalEmotes = EmoteEntry<SevenTv.Emote>.Empty;

    private readonly Dictionary<long, EmoteEntry<Ffz.Emote>> _ffzChannelsEmotes = new();
    private readonly Dictionary<long, EmoteEntry<Bttv.Emote>> _bttvChannelEmotes = new();
    private readonly Dictionary<long, EmoteEntry<SevenTv.Emote>> _sevenTvChannelEmotes = new();

    private static readonly TimeSpan _channelEmoteCacheTime = TimeSpan.FromHours(3);
    private static readonly TimeSpan _globalEmoteCacheTime = TimeSpan.FromDays(1);

    public string GetEmote(long channelId, string fallback, params string[] emotes)
    {
        int emoteCount = GetEmoteCount(channelId);
        using RentedArray<string> channelEmotesBuffer = ArrayPool<string>.Shared.Rent(emoteCount);
        GetAllEmoteNames(channelId, channelEmotesBuffer);
        Span<string> channelEmotes = channelEmotesBuffer[..emoteCount];
        using PoolBufferList<string> result = new(10);
        for (int i = 0; i < emotes.Length; i++)
        {
            Regex emotePattern = new(emotes[i], RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
            for (int j = 0; j < channelEmotes.Length; j++)
            {
                string emote = channelEmotes[j];
                if (emotePattern.IsMatch(emote))
                {
                    result.Add(emote);
                }
            }
        }

        return result.Count == 0 ? fallback : result.AsSpan().Random()!;
    }

    public int GetEmoteCount(long channelId)
    {
        return GetFfzEmotes(channelId).Length + GetBttvEmotes(channelId).Length + GetSevenTvEmotes(channelId).Length +
               FfzGlobalEmotes.Length + BttvGlobalEmotes.Length + SevenTvGlobalEmotes.Length;
    }

    public void GetAllEmoteNames(long channelId, Span<string> result)
    {
        ReadOnlySpan<Ffz.Emote> ffzEmotes = GetFfzEmotes(channelId);
        ReadOnlySpan<Bttv.Emote> bttvEmotes = GetBttvEmotes(channelId);
        ReadOnlySpan<SevenTv.Emote> sevenTvEmotes = GetSevenTvEmotes(channelId);

        ReadOnlySpan<Ffz.Emote> ffzGlobalEmotes = FfzGlobalEmotes;
        ReadOnlySpan<Bttv.Emote> bttvGlobalEmotes = BttvGlobalEmotes;
        ReadOnlySpan<SevenTv.Emote> sevenTvGlobalEmotes = SevenTvGlobalEmotes;

        int resultCount = 0;
        for (int i = 0; i < ffzEmotes.Length; i++)
        {
            result[resultCount++] = ffzEmotes[i].Name;
        }

        for (int i = 0; i < bttvEmotes.Length; i++)
        {
            result[resultCount++] = bttvEmotes[i].Name;
        }

        for (int i = 0; i < sevenTvEmotes.Length; i++)
        {
            result[resultCount++] = sevenTvEmotes[i].Name;
        }

        for (int i = 0; i < ffzGlobalEmotes.Length; i++)
        {
            result[resultCount++] = ffzGlobalEmotes[i].Name;
        }

        for (int i = 0; i < bttvGlobalEmotes.Length; i++)
        {
            result[resultCount++] = bttvGlobalEmotes[i].Name;
        }

        for (int i = 0; i < sevenTvGlobalEmotes.Length; i++)
        {
            result[resultCount++] = sevenTvGlobalEmotes[i].Name;
        }
    }

    public ReadOnlySpan<Ffz.Emote> GetFfzEmotes(long channelId, bool loadFromCache = true)
    {
        if (loadFromCache && _ffzChannelsEmotes.TryGetValue(channelId, out var emoteEntry) && emoteEntry.IsValid(_channelEmoteCacheTime))
        {
            return emoteEntry.Emotes;
        }

        Ffz.Response? response = GetFfzResponse(channelId);
        Ffz.Emote[]? emotes = response?.Set?.EmoteSet?.Emotes;
        emoteEntry = new(emotes);

        if (emoteEntry != EmoteEntry<Ffz.Emote>.Empty && !_ffzChannelsEmotes.TryAdd(channelId, emoteEntry))
        {
            _ffzChannelsEmotes[channelId] = emoteEntry;
        }

        return emoteEntry.Emotes;
    }

    public ReadOnlySpan<Bttv.Emote> GetBttvEmotes(long channelId, bool loadFromCache = true)
    {
        if (loadFromCache && _bttvChannelEmotes.TryGetValue(channelId, out var emoteEntry) && emoteEntry.IsValid(_channelEmoteCacheTime))
        {
            return emoteEntry.Emotes;
        }

        Bttv.Response? response = GetBttvResponse(channelId);
        Bttv.Emote[]? emotes = response?.ChannelEmotes?.Concat(response.SharedEmotes).ToArray();
        emoteEntry = new(emotes);

        if (emoteEntry != EmoteEntry<Bttv.Emote>.Empty && !_bttvChannelEmotes.TryAdd(channelId, emoteEntry))
        {
            _bttvChannelEmotes[channelId] = emoteEntry;
        }

        return emoteEntry.Emotes;
    }

    public ReadOnlySpan<SevenTv.Emote> GetSevenTvEmotes(long channelId, bool loadFromCache = true)
    {
        if (loadFromCache && _sevenTvChannelEmotes.TryGetValue(channelId, out var emoteEntry) && emoteEntry.IsValid(_channelEmoteCacheTime))
        {
            return emoteEntry.Emotes;
        }

        SevenTv.User? user = GetSevenTvUser(channelId);
        SevenTv.Emote[]? emotes = user?.EmoteSet?.Emotes;
        emoteEntry = new(emotes);

        if (emoteEntry != EmoteEntry<SevenTv.Emote>.Empty && !_sevenTvChannelEmotes.TryAdd(channelId, emoteEntry))
        {
            _sevenTvChannelEmotes[channelId] = emoteEntry;
        }

        return emoteEntry.Emotes;
    }

    private static SevenTv.User? GetSevenTvUser(long channelId)
    {
        try
        {
            HttpGet request = new($"https://7tv.io/v3/users/twitch/{channelId}");
            return string.IsNullOrWhiteSpace(request.Result) ? null : JsonSerializer.Deserialize<SevenTv.User>(request.Result);
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private static SevenTv.Emote[]? GetSevenTvGlobalEmotes()
    {
        try
        {
            HttpGet request = new("https://7tv.io/v3/emote-sets/global");
            SevenTv.EmoteSet? emoteSet = string.IsNullOrWhiteSpace(request.Result) ? null : JsonSerializer.Deserialize<SevenTv.EmoteSet>(request.Result);
            return emoteSet?.Emotes;
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private static Bttv.Emote[]? GetBttvGlobalEmotes()
    {
        try
        {
            HttpGet request = new("https://api.betterttv.net/3/cached/emotes/global");
            return request.Result is null ? null : JsonSerializer.Deserialize<Bttv.Emote[]>(request.Result);
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private static Bttv.Response? GetBttvResponse(long channelId)
    {
        try
        {
            HttpGet request = new($"https://api.betterttv.net/3/cached/users/twitch/{channelId}");
            return request.Result is null ? null : JsonSerializer.Deserialize<Bttv.Response>(request.Result);
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private Ffz.Response? GetFfzResponse(long channelId)
    {
        try
        {
            HttpGet request = new($"https://api.frankerfacez.com/v1/room/id/{channelId}");
            if (request.Result is null)
            {
                return null;
            }

            JsonElement json = JsonSerializer.Deserialize<JsonElement>(request.Result);
            int setId = json.GetProperty("room").GetProperty("set").GetInt32();
            string result = request.Result.Replace($"\"{setId}\":", "\"mainSet\":");
            return JsonSerializer.Deserialize<Ffz.Response>(result);
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private static Ffz.Emote[]? GetFfzGlobalEmotes()
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
            string secondSet = json.GetProperty("sets").GetProperty("mainSet").GetProperty("emoticons").GetRawText();
            Ffz.Emote[] firstEmoteSet = JsonSerializer.Deserialize<Ffz.Emote[]>(firstSet) ?? Array.Empty<Ffz.Emote>();
            Ffz.Emote[] secondEmoteSet = JsonSerializer.Deserialize<Ffz.Emote[]>(secondSet) ?? Array.Empty<Ffz.Emote>();
            Ffz.Emote[] emotes = firstEmoteSet.Concat(secondEmoteSet).ToArray();
            return emotes;
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }
}
