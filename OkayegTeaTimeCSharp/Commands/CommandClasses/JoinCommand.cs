using HLE.Strings;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class JoinCommand : Command
    {
        public JoinCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\s#?\w+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendJoinChannel(TwitchBot, ChatMessage));
            }
        }
    }
}
