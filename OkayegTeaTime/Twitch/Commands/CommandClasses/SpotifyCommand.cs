using System.Text.RegularExpressions;
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
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSpotifySearch(ChatMessage));
            return;
        }

        Regex currentPlayingPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"(\s\w+)?");
        if (currentPlayingPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSpotifyCurrentlyPlaying(ChatMessage));
        }
    }
}
