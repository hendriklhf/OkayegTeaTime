using HLE.Strings;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class UnsetCommand : Command
    {
        public UnsetCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\sprefix")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendUnsetPrefix(ChatMessage));
            }
            else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\sreminder\s\d+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendUnsetReminder(ChatMessage));
            }
            else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\semote")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendUnsetEmoteInFront(ChatMessage));
            }
            else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\snuke\s\d+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendUnsetNuke(ChatMessage));
            }
        }
    }
}
