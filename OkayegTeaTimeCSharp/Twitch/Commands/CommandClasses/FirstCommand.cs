using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses
{
    public class FirstCommand : Command
    {
        public FirstCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixHelper.GetPrefix(ChatMessage.Channel), @"\s\w+\s#?\w+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendFirstUserChannel(ChatMessage));
            }
            else if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixHelper.GetPrefix(ChatMessage.Channel), @"\s#\w+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendFirstChannel(ChatMessage));
            }
            else if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixHelper.GetPrefix(ChatMessage.Channel), @"\s\w+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendFirstUser(ChatMessage));
            }
            else if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixHelper.GetPrefix(ChatMessage.Channel))))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendFirst(ChatMessage));
            }
        }
    }
}