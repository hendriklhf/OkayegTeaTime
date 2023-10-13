using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Collections;
using HLE.Emojis;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public readonly struct RedditCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<RedditCommand>
{
    public PooledStringBuilder Response { get; } = new(AppSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    private static readonly Dictionary<string, RedditPost[]> _redditPostCache = new();
    private static readonly TimeSpan _cacheTime = TimeSpan.FromHours(1);
    private static readonly Func<RedditPost, bool> _postFilter = static rp => rp is { Pinned: false, IsNsfw: false };

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out RedditCommand command)
    {
        command = new(twitchBot, chatMessage, prefix, alias);
    }

    public async ValueTask Handle()
    {
        Regex pattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            string subReddit = StringPool.Shared.GetOrAdd(messageExtension.LowerSplit[1].Span);
            RedditPost[]? posts = await GetRedditPostsAsync(subReddit);
            if (posts is null)
            {
                Response.Append(ChatMessage.Username, ", ", Messages.ApiError);
                return;
            }

            RedditPost? post = posts.Random();
            if (post is null)
            {
                Response.Append(ChatMessage.Username, ", ", Messages.ThereAreNoPostsAvailable);
                return;
            }

            Response.Append(ChatMessage.Username, ", ");
            CheckForNsfwAndSpoiler(post);
            Response.Append(post.Title, " ", post.Url, " Score: ");
            Response.Append(post.Score);
        }
    }

    private static async ValueTask<RedditPost[]?> GetRedditPostsAsync(string subReddit)
    {
        try
        {
            if (_redditPostCache.TryGetValue(subReddit, out RedditPost[]? redditPosts) && redditPosts[0].IsValid(_cacheTime))
            {
                return redditPosts;
            }

            HttpGet request = await HttpGet.GetStringAsync($"https://www.reddit.com/r/{subReddit}/hot.json?limit=50");
            if (request.Result is null)
            {
                return null;
            }

            JsonElement json = JsonSerializer.Deserialize<JsonElement>(request.Result);
            JsonElement posts = json.GetProperty("data").GetProperty("children");
            using PooledList<string> rawPosts = new(50);
            for (int i = 0; i < posts.GetArrayLength(); i++)
            {
                rawPosts.Add(posts[i].GetProperty("data").GetRawText());
            }

            redditPosts = JsonSerializer.Deserialize<RedditPost[]>('[' + rawPosts.JoinToString(',') + ']')?.Where(_postFilter).ToArray();
            if (redditPosts is null)
            {
                return null;
            }

            if (!_redditPostCache.TryAdd(subReddit, redditPosts))
            {
                _redditPostCache[subReddit] = redditPosts;
            }

            return redditPosts;
        }
        catch (Exception ex)
        {
            await DbController.LogExceptionAsync(ex);
            return null;
        }
    }

    private void CheckForNsfwAndSpoiler(RedditPost post)
    {
        if (post.IsNsfw)
        {
            Response.Append(Emoji.Warning, " NSFW 18+ ");
            if (post.IsSpoiler)
            {
                Response.Append(Emoji.Warning, " Spoiler ");
            }

            Response.Append(Emoji.Warning, " ");
        }
        else if (post.IsSpoiler)
        {
            Response.Append(Emoji.Warning, " Spoiler ", Emoji.Warning);
        }
    }

    public void Dispose()
    {
        Response.Dispose();
    }

    public bool Equals(RedditCommand other)
    {
        return _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) && Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);
    }

    public override bool Equals(object? obj)
    {
        return obj is RedditCommand other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);
    }

    public static bool operator ==(RedditCommand left, RedditCommand right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RedditCommand left, RedditCommand right)
    {
        return !left.Equals(right);
    }
}
