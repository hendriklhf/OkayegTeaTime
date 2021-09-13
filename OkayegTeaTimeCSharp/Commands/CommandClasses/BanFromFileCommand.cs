using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class BanFromFileCommand : Command
    {
        public BanFromFileCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixDictionary.Get(ChatMessage.Channel), @"\s\S+\s\S+")))
            {
                BotActions.SendBanFromFile(TwitchBot, ChatMessage);
            }
        }
    }
}
