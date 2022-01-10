using System.Text.RegularExpressions;
using HLE.Collections;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class SpotifyCommand : Command
{
    public SpotifyCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex searchPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\ssearch\s.+");
        if (searchPattern.IsMatch(ChatMessage.Message))
        {
            string query = ChatMessage.Split.Skip(2).JoinToString(' ');
            Response = $"{ChatMessage.Username}, {SpotifyRequest.Search(query).Result}";
            return;
        }

        Regex currentPlayingPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"(\s\w+)?");
        if (currentPlayingPattern.IsMatch(ChatMessage.Message))
        {
            string username = ChatMessage.LowerSplit.Length > 1
                ? (ChatMessage.LowerSplit[1] == "me"
                    ? ChatMessage.Username
                    : ChatMessage.LowerSplit[1])
                : ChatMessage.Channel.Name;
            Response = $"{ChatMessage.Username}, {SpotifyRequest.GetCurrentlyPlaying(username).Result}";
        }
    }
}
