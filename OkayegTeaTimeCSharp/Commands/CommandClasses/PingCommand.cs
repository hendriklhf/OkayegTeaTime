using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;
using OkayegTeaTimeCSharp.Prefixes;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Utils;
using OkayegTeaTimeCSharp.Commands.CommandEnums;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class PingCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage)
        {
            twitchBot.Send(chatMessage.Channel, $"Pongeg, I'm here! Uptime: {twitchBot.Runtime}");
        }
    }
}
