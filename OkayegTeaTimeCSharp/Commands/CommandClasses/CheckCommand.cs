using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class CheckCommand : Command
    {
        public CheckCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\safk\s\w+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckAfk(ChatMessage));
            }
            else if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\sreminder\s\d+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckReminder(ChatMessage));
            }
            else if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\smessages?\s\d+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckMessage(ChatMessage));
            }
        }
    }
}
