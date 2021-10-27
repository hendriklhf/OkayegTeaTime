using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

public class SuggestCommand : Command
{
    public SuggestCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S{3,}")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSuggestionNoted(ChatMessage));
        }
    }
}
