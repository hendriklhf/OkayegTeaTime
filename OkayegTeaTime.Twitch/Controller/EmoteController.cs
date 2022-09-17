using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using HLE.Http;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Cache;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.Models;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class EmoteController
{
    public IEnumerable<FfzEmote> FfzGlobalEmotes
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

    public IEnumerable<BttvEmote> BttvGlobalEmotes
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

    public IEnumerable<SevenTvGlobalEmote> SevenTvGlobalEmotes
    {
        get
        {
            if (_sevenTvGlobalEmotes is not null)
            {
                return _sevenTvGlobalEmotes;
            }

            _sevenTvGlobalEmotes = GetSevenTvGlobalEmotes();
            return _sevenTvGlobalEmotes ?? Array.Empty<SevenTvGlobalEmote>();
        }
    }

    private readonly ChannelCache? _channelCache;

    private IEnumerable<FfzEmote>? _ffzGlobalEmotes;
    private IEnumerable<BttvEmote>? _bttvGlobalEmotes;
    private IEnumerable<SevenTvGlobalEmote>? _sevenTvGlobalEmotes;

    private readonly Dictionary<long, IEnumerable<FfzEmote>> _ffzChannelsEmotes = new();
    private readonly Dictionary<long, IEnumerable<BttvEmote>> _bttvEmotes = new();
    private readonly Dictionary<long, IEnumerable<SevenTvEmote>> _sevenTvChannelEmotes = new();

    public EmoteController(ChannelCache? channelCache = null)
    {
        _channelCache = channelCache;
    }

    public IEnumerable<FfzEmote> GetFfzEmotes(long channelId, bool loadFromCache = true)
    {
        if (loadFromCache && _ffzChannelsEmotes.TryGetValue(channelId, out IEnumerable<FfzEmote>? result))
        {
            return result;
        }

        FfzRequest? request = GetFfzRequest(channelId);
        IEnumerable<FfzEmote>? emotes = request?.Set?.EmoteSet?.Emotes;
        if (emotes is not null && !_ffzChannelsEmotes.TryAdd(channelId, emotes))
        {
            _ffzChannelsEmotes[channelId] = emotes;
        }

        return emotes ?? Array.Empty<FfzEmote>();
    }

    public IEnumerable<BttvEmote> GetBttvEmotes(long channelId, bool loadFromCache = true)
    {
        if (loadFromCache && _bttvEmotes.TryGetValue(channelId, out IEnumerable<BttvEmote>? result))
        {
            return result;
        }

        BttvRequest? request = GetBttvRequest(channelId);
        BttvEmote[]? emotes = request?.ChannelEmotes?.Concat(request.SharedEmotes).ToArray();
        if (emotes is not null && !_bttvEmotes.TryAdd(channelId, emotes))
        {
            _bttvEmotes[channelId] = emotes;
        }

        return emotes ?? Array.Empty<BttvEmote>();
    }

    public IEnumerable<SevenTvEmote> GetSevenTvEmotes(long channelId, bool loadFromCache = true)
    {
        if (loadFromCache && _sevenTvChannelEmotes.TryGetValue(channelId, out IEnumerable<SevenTvEmote>? result))
        {
            return result;
        }

        SevenTvRequest? request = GetSevenTvRequest(channelId);
        SevenTvEmote[]? emotes = request?.Data?.User?.Emotes;
        if (emotes is not null && !_sevenTvChannelEmotes.TryAdd(channelId, emotes))
        {
            _sevenTvChannelEmotes[channelId] = emotes;
        }

        return emotes ?? Array.Empty<SevenTvEmote>();
    }

    private SevenTvRequest? GetSevenTvRequest(long channelId)
    {
        try
        {
            string? channelName = _channelCache is null ? DbController.GetChannel(channelId)?.Name : _channelCache[channelId]?.Name;
            if (channelName is null)
            {
                return null;
            }

            HttpPost request = new("https://api.7tv.app/v2/gql", new[]
            {
                ("query",
                    "{user(id: \"" + channelName + "\") {...FullUser}}fragment FullUser on User {id,email, display_name, login,description,role " +
                    "{id,name,position,color,allowed,denied},emotes { id, name, status, visibility, width, height },owned_emotes { id, name, status, visibility, width, height }," +
                    "emote_ids,editor_ids,editors {id, display_name, login,role { id, name, position, color, allowed, denied },profile_image_url,emote_ids},editor_in {id, display_name, " +
                    "login,role { id, name, position, color, allowed, denied },profile_image_url,emote_ids},twitch_id,broadcaster_type,profile_image_url,created_at}")
            });

            return request.Result is null ? null : JsonSerializer.Deserialize<SevenTvRequest>(request.Result);
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private static IEnumerable<SevenTvGlobalEmote>? GetSevenTvGlobalEmotes()
    {
        try
        {
            HttpPost request = new("https://api.7tv.app/v2/gql", new[]
            {
                ("query", "{search_emotes(query: \"\", globalState: \"only\", page: 1, limit: 150, pageSize: 150) {id,name,provider,provider_id,visibility,mime,owner {id,display_name,login,twitch_id}}}")
            });

            if (!request.IsValidJsonData)
            {
                return null;
            }

            string result = request.Data.GetProperty("data").GetProperty("search_emotes").GetRawText();
            return JsonSerializer.Deserialize<SevenTvGlobalEmote[]>(result);
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private static IEnumerable<BttvEmote>? GetBttvGlobalEmotes()
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
            if (!request.IsValidJsonData || request.Result is null)
            {
                return null;
            }

            int setId = request.Data.GetProperty("room").GetProperty("set").GetInt32();
            string result = request.Result.Replace($"\"{setId}\":", $"\"{AppSettings.FfzSetIdReplacement}\":");
            return JsonSerializer.Deserialize<FfzRequest>(result);
        }
        catch (Exception ex)
        {
            DbController.LogException(ex);
            return null;
        }
    }

    private static IEnumerable<FfzEmote>? GetFfzGlobalEmotes()
    {
        try
        {
            HttpGet request = new("https://api.frankerfacez.com/v1/set/global");
            if (!request.IsValidJsonData || request.Result is null)
            {
                return null;
            }

            int setId = request.Data.GetProperty("default_sets")[0].GetInt32();
            string result = request.Result.Replace($"\"{setId}\":", $"\"{AppSettings.FfzSetIdReplacement}\":");
            JsonElement json = JsonSerializer.Deserialize<JsonElement>(result);
            string firstSet = json.GetProperty("sets").GetProperty(AppSettings.FfzSetIdReplacement).GetProperty("emoticons").GetRawText();
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
