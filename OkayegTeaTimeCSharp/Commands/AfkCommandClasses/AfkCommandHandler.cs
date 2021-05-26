using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;
using OkayegTeaTimeCSharp.Commands.CommandEnums;

namespace OkayegTeaTimeCSharp.Commands.AfkCommandClasses
{
    public static class AfkCommandHandler
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, AfkCommandType type)
        {
            twitchBot.SendGoingAfk(chatMessage, type);
        }
    }
}