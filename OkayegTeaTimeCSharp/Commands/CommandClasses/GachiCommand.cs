using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class GachiCommand : Command
    {
        public GachiCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendRandomGachi());
        }
    }
}