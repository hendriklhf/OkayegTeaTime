using HLE.Strings;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class CheckCommand : Command
    {
        public CheckCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\safk\s\w+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckAfk(ChatMessage));
            }
            else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\sreminder\s\d+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckReminder(ChatMessage));
            }
            else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\smessages?\s\d+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendCheckMessage(ChatMessage));
            }
        }
    }
}
