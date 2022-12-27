using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using HLE.Collections;
using HLE.Emojis;
using HLE.Http;
using OkayegTeaTime.Database;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Reddit)]
public readonly unsafe ref struct RedditCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public Response* Response { get; }

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private static readonly Dictionary<string, RedditPost[]> _redditPosts = new();
    private static readonly TimeSpan _cacheTime = TimeSpan.FromHours(1);
    private static readonly Func<RedditPost, bool> _postFilter = rp => rp is { Pinned: false, IsNsfw: false };

    public RedditCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, Response* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            RedditPost[]? posts = GetRedditPosts(ChatMessage.LowerSplit[1]);
            if (posts is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.ApiError);
                return;
            }

            RedditPost? post = posts.Random();
            if (post is null)
            {
                Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace, PredefinedMessages.ThereAreNoPostsAvailable);
                return;
            }

            Response->Append(ChatMessage.Username, PredefinedMessages.CommaSpace);
            CheckForNsfwAndSpoiler(post);
            Response->Append(post.Title, StringHelper.Whitespace, post.Url, " Score: ", post.Score.ToString());
        }
    }

    private static RedditPost[]? GetRedditPosts(string subReddit)
    {
        try
        {
            if (_redditPosts.TryGetValue(subReddit, out RedditPost[]? redditPosts) && redditPosts[0].TimeOfRequest + _cacheTime > DateTime.UtcNow)
            {
                return redditPosts;
            }

            HttpGet request = new($"https://www.reddit.com/r/{subReddit}/hot.json?limit=50");
            if (request.Result is null || !request.IsValidJsonData)
            {
                return null;
            }

            JsonElement posts = request.Data.GetProperty("data").GetProperty("children");
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

            if (!_redditPosts.TryAdd(subReddit, redditPosts))
            {
                _redditPosts[subReddit] = redditPosts;
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
            Response->Append(Emoji.Warning, " NSFW 18+ ");
            if (post.IsSpoiler)
            {
                Response->Append(Emoji.Warning, " Spoiler ");
            }

            Response->Append(Emoji.Warning, StringHelper.Whitespace);
        }
        else if (post.IsSpoiler)
        {
            Response->Append(Emoji.Warning, " Spoiler ", Emoji.Warning);
        }
    }
}
