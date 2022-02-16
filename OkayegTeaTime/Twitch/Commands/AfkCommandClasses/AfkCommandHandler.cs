using HLE.Collections;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.AfkCommandClasses;

public class AfkCommandHandler
{
    public TwitchBot TwitchBot { get; }

    public AfkCommandHandler(TwitchBot twitchBot)
    {
        TwitchBot = twitchBot;
    }

    public void Handle(TwitchChatMessage chatMessage, AfkCommandType type)
    {
        UserNew? user = DbController.GetUser(chatMessage.UserId, chatMessage.Username);
        if (user is null)
        {
            DbController.AddUser(chatMessage.UserId, chatMessage.Username, type);
        }

        string message = chatMessage.Split[1..].JoinToString(' ');
        DbController.SetAfk(chatMessage.UserId, message, type);

        user = DbController.GetUser(chatMessage.UserId);
        if (user is null)
        {
            return;
        }

        AfkMessage afkMessage = new(chatMessage.UserId);
        if (afkMessage.GoingAway is null)
        {
            return;
        }
        TwitchBot.Send(chatMessage.Channel, afkMessage.GoingAway);
    }
}
