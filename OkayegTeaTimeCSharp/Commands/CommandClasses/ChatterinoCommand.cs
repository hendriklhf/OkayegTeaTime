using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class ChatterinoCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            twitchBot.Send(chatMessage.Channel, $"Website: chatterino.com || Releases: github.com/Chatterino/chatterino2/releases");
        }
    }
}
