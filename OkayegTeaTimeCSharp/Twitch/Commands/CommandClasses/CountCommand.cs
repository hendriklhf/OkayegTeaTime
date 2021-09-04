using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses
{
    public class CountCommand : Command
    {
        public CountCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\s#\w+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendLoggedMessagesChannelCount(ChatMessage));
            }
            else if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\s\w+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendLoggedMessagesUserCount(ChatMessage));
            }
            else if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel))))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendLoggedMessagesCount(ChatMessage));
            }
        }
    }
}
