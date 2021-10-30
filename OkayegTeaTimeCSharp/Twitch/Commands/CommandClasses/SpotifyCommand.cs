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
        if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\ssearch\s.+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSpotifySearch(ChatMessage));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"(\s\w+)?")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSpotifyCurrentlyPlaying(ChatMessage));
        }
    }
}
