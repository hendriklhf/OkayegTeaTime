using HLE.Strings;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class CountCommand : Command
    {
        public CountCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\s#\w+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendLoggedMessagesChannelCount(ChatMessage));
            }
            else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\s\w+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendLoggedMessagesUserCount(ChatMessage));
            }
            else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel))))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendLoggedMessagesCount(ChatMessage));
            }
        }
    }
}
