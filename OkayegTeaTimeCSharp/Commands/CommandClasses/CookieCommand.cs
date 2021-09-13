using OkayegTeaTimeCSharp.Twitch.Bot;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class CookieCommand : Command
    {
        public CookieCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendRandomCookie(ChatMessage));
        }
    }
}
