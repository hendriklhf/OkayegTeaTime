using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Commands.CommandEnums;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Commands.AfkCommandClasses
{
    public static class AfkCommandHandler
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, AfkCommandType type)
        {
            twitchBot.SendGoingAfk(chatMessage, type);
        }
    }
}