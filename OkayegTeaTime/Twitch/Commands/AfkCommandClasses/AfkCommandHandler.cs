using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.AfkCommandClasses;

public static class AfkCommandHandler
{
    public static void Handle(TwitchBot twitchBot, ITwitchChatMessage chatMessage, AfkCommandType type)
    {
        twitchBot.SendGoingAfk(chatMessage, type);
    }
}
