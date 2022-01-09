using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.AfkCommandClasses;

public static class AfkCommandHandler
{
    public static void Handle(TwitchBot twitchBot, TwitchChatMessage chatMessage, AfkCommandType type)
    {
        twitchBot.SendGoingAfk(chatMessage, type);
    }
}
