using HLE.Strings;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class EmoteCommand : Command
    {
        public EmoteCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\sffz(\s((\d+)|(\w+(\s\d+)?)))?")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendFFZEmotes(ChatMessage));
            }
            else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\sbttv(\s((\d+)|(\w+(\s\d+)?)))?")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendBTTVEmotes(ChatMessage));
            }
            else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\s7tv(\s((\d+)|(\w+(\s\d+)?)))?")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.Send7TVEmotes(ChatMessage));
            }
        }
    }
}
