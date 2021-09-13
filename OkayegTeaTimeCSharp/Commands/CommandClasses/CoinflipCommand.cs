using OkayegTeaTimeCSharp.Twitch.Bot;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class CoinflipCommand : Command
    {
        public CoinflipCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendCoinFlip(ChatMessage));
        }
    }
}
