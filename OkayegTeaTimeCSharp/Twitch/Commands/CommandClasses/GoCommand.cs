using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

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
