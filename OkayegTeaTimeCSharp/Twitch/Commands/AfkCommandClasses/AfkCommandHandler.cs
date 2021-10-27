using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Commands.Enums;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands.AfkCommandClasses;

public static class AfkCommandHandler
{
    public static void Handle(TwitchBot twitchBot, ITwitchChatMessage chatMessage, AfkCommandType type)
    {
        twitchBot.SendGoingAfk(chatMessage, type);
    }
}
