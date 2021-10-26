using OkayegTeaTimeCSharp.Commands.Enums;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;

namespace OkayegTeaTimeCSharp.Commands.AfkCommandClasses;

public static class AfkCommandHandler
{
    public static void Handle(TwitchBot twitchBot, ITwitchChatMessage chatMessage, AfkCommandType type)
    {
        twitchBot.SendGoingAfk(chatMessage, type);
    }
}
