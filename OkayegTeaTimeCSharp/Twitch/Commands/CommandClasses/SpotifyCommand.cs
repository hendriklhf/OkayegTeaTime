using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

public class SpotifyCommand : Command
{
    public SpotifyCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        var searchPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\ssearch\s.+");
        if (searchPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSpotifySearch(ChatMessage));
            return;
        }

        var currentPlayingPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"(\s\w+)?");
        if (currentPlayingPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSpotifyCurrentlyPlaying(ChatMessage));
        }
    }
}
