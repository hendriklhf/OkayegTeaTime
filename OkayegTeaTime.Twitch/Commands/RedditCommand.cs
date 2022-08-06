using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using HLE.Collections;
using HLE.Emojis;
using HLE.Http;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Reddit)]
public class RedditCommand : Command
{
    private static readonly Dictionary<string, RedditPost[]> _redditPosts = new();
    private static readonly long _cacheTime = (long)TimeSpan.FromHours(1).TotalMilliseconds;

    public RedditCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s\S+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            RedditPost[]? posts = GetRedditPosts(ChatMessage.LowerSplit[1]);
            if (posts is null)
            {
                Response = $"{ChatMessage.Username}, api error";
                return;
            }

            RedditPost? post = posts.Random();
            if (post is null)
            {
                Response = $"{ChatMessage.Username}, there are no posts available";
                return;
            }

            Response += $"{ChatMessage.Username}, ";
            CheckForNsfwAndSpoiler(post);
            Response += $"{post.Title} {post.Url} Score: {post.Score}";
        }
    }

    private static RedditPost[]? GetRedditPosts(string subReddit)
    {
        try
        {
            if (_redditPosts.TryGetValue(subReddit, out RedditPost[]? redditPosts) && redditPosts[0].TimeOfRequest + _cacheTime > TimeHelper.Now())
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

            redditPosts = JsonSerializer.Deserialize<RedditPost[]>('[' + rawPosts.JoinToString(',') + ']')?.Where(p => !p.Pinned && !p.IsNsfw).ToArray();
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
            Response += $"{Emoji.Warning} NSFW 18+ ";
            if (post.IsSpoiler)
            {
                Response += $"{Emoji.Warning} Spoiler ";
            }

            Response += $"{Emoji.Warning} ";
        }
        else if (post.IsSpoiler)
        {
            Response += $"{Emoji.Warning} Spoiler {Emoji.Warning}";
        }
    }
}
