using HLE.Strings;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses;

public class FillCommand : Command
{
    public FillCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\S+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendFill(ChatMessage));
        }
    }
}
