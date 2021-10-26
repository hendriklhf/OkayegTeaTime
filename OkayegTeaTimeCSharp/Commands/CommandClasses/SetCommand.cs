using HLE.Strings;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses;

public class SetCommand : Command
{
    public SetCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\sprefix\s\S+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetPrefix(ChatMessage));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\semote\s\S+")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetEmoteInFront(ChatMessage));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s(sr|songrequests?)\s((1|true|enabled?)|(0|false|disabled?))")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetSongRequestState(ChatMessage));
        }
    }
}
