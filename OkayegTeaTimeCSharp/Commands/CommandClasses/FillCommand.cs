using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class FillCommand : Command
    {
        public FillCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\s\S+")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendFill(ChatMessage));
            }
        }
    }
}
