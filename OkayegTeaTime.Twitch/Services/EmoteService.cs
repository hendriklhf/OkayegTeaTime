using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Strings;

namespace OkayegTeaTime.Twitch.Services;

public sealed class EmoteService(TwitchBot twitchBot) : IEquatable<EmoteService>
{
    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ConcurrentDictionary<long, CacheEntry<StringArray>> _emoteNamesCache = new();
    private readonly TimeSpan _emoteNamesCacheTime = TimeSpan.FromHours(1);

    public async ValueTask<string> GetBestEmoteAsync(long channelId, string fallback, params string[] keywords)
    {
        StringArray emoteNames = await GetAllEmoteNamesAsync(channelId);
        using PooledList<string> bestEmotes = new(32);
        foreach (string keyword in keywords)
        {
            for (int i = 0; i < emoteNames.Length; i++)
            {
                AddEmoteIfContainsKeyword(emoteNames, i, keyword, bestEmotes);
            }
        }

        return bestEmotes.AsSpan().Random() ?? fallback;
    }

    private static void AddEmoteIfContainsKeyword(StringArray emoteNames, int i, string keyword, PooledList<string> bestEmotes)
    {
        ReadOnlySpan<char> emoteName = emoteNames.GetChars(i);
        if (emoteName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        {
            bestEmotes.Add(emoteNames[i]);
        }
    }

    public async ValueTask<StringArray> GetAllEmoteNamesAsync(long channelId)
    {
        if (_emoteNamesCache.TryGetValue(channelId, out CacheEntry<StringArray> emoteNamesEntry) && emoteNamesEntry.IsValid(_emoteNamesCacheTime))
        {
            return emoteNamesEntry.Value;
        }

        var getTwitchGlobalEmotesTask = _twitchBot.TwitchApi.GetGlobalEmotesAsync().AsTask();
        var getTwitchChannelEmotesTask = _twitchBot.TwitchApi.GetChannelEmotesAsync(channelId).AsTask();
        var getFfzGlobalEmotesTask = _twitchBot.FfzApi.GetGlobalEmotesAsync().AsTask();
        var getFfzChannelEmotesTask = _twitchBot.FfzApi.GetChannelEmotesAsync(channelId).AsTask();
        var getBttvGlobalEmotesTask = _twitchBot.BttvApi.GetGlobalEmotesAsync().AsTask();
        var getBttvChannelEmotesTask = _twitchBot.BttvApi.GetChannelEmotesAsync(channelId).AsTask();
        var getSevenTvGlobalEmotesTask = _twitchBot.SevenTvApi.GetGlobalEmotesAsync().AsTask();
        var getSevenTvChannelEmotesTask = _twitchBot.SevenTvApi.GetChannelEmotesAsync(channelId).AsTask();

        await Task.WhenAll(getTwitchGlobalEmotesTask, getTwitchChannelEmotesTask, getFfzGlobalEmotesTask, getFfzChannelEmotesTask,
            getBttvGlobalEmotesTask, getBttvChannelEmotesTask, getSevenTvGlobalEmotesTask, getSevenTvChannelEmotesTask);

#pragma warning disable CA1849
        var twitchGlobalEmotes = getTwitchGlobalEmotesTask.Result;
        var twitchChannelEmotes = getTwitchChannelEmotesTask.Result;
        var ffzGlobalEmotes = getFfzGlobalEmotesTask.Result;
        var ffzChannelEmotes = getFfzChannelEmotesTask.Result;
        var bttvGlobalEmotes = getBttvGlobalEmotesTask.Result;
        var bttvChannelEmotes = getBttvChannelEmotesTask.Result;
        var sevenTvGlobalEmotes = getSevenTvGlobalEmotesTask.Result;
        var sevenTvChannelEmotes = getSevenTvChannelEmotesTask.Result;
#pragma warning restore CA1849

        int totalEmoteCount = twitchGlobalEmotes.Length + twitchChannelEmotes.Length + ffzGlobalEmotes.Length + (ffzChannelEmotes?.Length ?? 0) +
                              bttvGlobalEmotes.Length + (bttvChannelEmotes?.Length ?? 0) + sevenTvGlobalEmotes.Length + (sevenTvChannelEmotes?.Length ?? 0);

        StringArray emoteNames = new(totalEmoteCount);
        int emoteNameCount = 0;

        foreach (var emote in twitchGlobalEmotes)
        {
            emoteNames[emoteNameCount++] = emote.Name;
        }

        foreach (var emote in twitchChannelEmotes)
        {
            emoteNames[emoteNameCount++] = emote.Name;
        }

        foreach (var emote in ffzGlobalEmotes)
        {
            emoteNames[emoteNameCount++] = emote.Name;
        }

        if (ffzChannelEmotes is not null)
        {
            foreach (var emote in ffzChannelEmotes)
            {
                emoteNames[emoteNameCount++] = emote.Name;
            }
        }

        foreach (var emote in bttvGlobalEmotes)
        {
            emoteNames[emoteNameCount++] = emote.Name;
        }

        if (bttvChannelEmotes is not null)
        {
            foreach (var emote in bttvChannelEmotes)
            {
                emoteNames[emoteNameCount++] = emote.Name;
            }
        }

        foreach (var emote in sevenTvGlobalEmotes)
        {
            emoteNames[emoteNameCount++] = emote.Name;
        }

        if (sevenTvChannelEmotes is not null)
        {
            foreach (var emote in sevenTvChannelEmotes)
            {
                emoteNames[emoteNameCount++] = emote.Name;
            }
        }

        _emoteNamesCache.AddOrSet(channelId, new(emoteNames));
        return emoteNames;
    }

    public bool Equals(EmoteService? other)
    {
        return ReferenceEquals(this, other);
    }

    public override bool Equals(object? obj)
    {
        return obj is EmoteService other && Equals(other);
    }

    public override int GetHashCode()
    {
        return RuntimeHelpers.GetHashCode(this);
    }

    public static bool operator ==(EmoteService? left, EmoteService? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(EmoteService? left, EmoteService? right)
    {
        return !(left == right);
    }
}
