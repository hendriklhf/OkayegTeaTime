using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using HLE.Collections;
using HLE.Emojis;
using HLE.Twitch;
using HLE.Twitch.Models;
using OkayegTeaTime.Database;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Reddit)]
public readonly ref struct RedditCommand
{
    public ChatMessage ChatMessage { get; }

    private readonly ref MessageBuilder _response;

    private readonly TwitchBot _twitchBot;
    private readonly ReadOnlySpan<char> _prefix;
    private readonly ReadOnlySpan<char> _alias;

    private static readonly Dictionary<string, RedditPost[]> _redditPostCache = new();
    private static readonly TimeSpan _cacheTime = TimeSpan.FromHours(1);
    private static readonly Func<RedditPost, bool> _postFilter = rp => rp is { Pinned: false, IsNsfw: false };

    public RedditCommand(TwitchBot twitchBot, ChatMessage chatMessage, ref MessageBuilder response, ReadOnlySpan<char> prefix, ReadOnlySpan<char> alias)
    {
        ChatMessage = chatMessage;
        _response = ref response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            using ChatMessageExtension messageExtension = new(ChatMessage);
            RedditPost[]? posts = GetRedditPosts(new(messageExtension.LowerSplit[1]));
            if (posts is null)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.ApiError);
                return;
            }

            RedditPost? post = posts.Random();
            if (post is null)
            {
                _response.Append(ChatMessage.Username, ", ", Messages.ThereAreNoPostsAvailable);
                return;
            }

            _response.Append(ChatMessage.Username, ", ");
            CheckForNsfwAndSpoiler(post);
            _response.Append(post.Title, " ", post.Url, " Score: ");
            _response.Append(post.Score);
        }
    }

    private static RedditPost[]? GetRedditPosts(string subReddit)
    {
        try
        {
            if (_redditPostCache.TryGetValue(subReddit, out RedditPost[]? redditPosts) && redditPosts[0].TimeOfRequest + _cacheTime > DateTime.UtcNow)
            {
                return redditPosts;
            }

            HttpGet request = new("https://www.reddit.com/r/" + subReddit + "/hot.json?limit=50");
            if (request.Result is null)
            {
                return null;
            }

            JsonElement json = JsonSerializer.Deserialize<JsonElement>(request.Result);
            JsonElement posts = json.GetProperty("data").GetProperty("children");
            List<string> rawPosts = new();
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
            DbController.LogException(ex);
            return null;
        }
    }

    private void CheckForNsfwAndSpoiler(RedditPost post)
    {
        if (post.IsNsfw)
        {
            _response.Append(Emoji.Warning, " NSFW 18+ ");
            if (post.IsSpoiler)
            {
                _response.Append(Emoji.Warning, " Spoiler ");
            }

            _response.Append(Emoji.Warning, " ");
        }
        else if (post.IsSpoiler)
        {
            _response.Append(Emoji.Warning, " Spoiler ", Emoji.Warning);
        }
    }
}
