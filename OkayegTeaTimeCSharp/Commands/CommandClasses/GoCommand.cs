using HLE.Strings;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses;

public class GoCommand : Command
{
    public GoCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s.+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendGoLangCompilerResult(ChatMessage));
        }
    }
}
