using HLE.Collections;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands.Enums;
using OkayegTeaTime.Twitch.Models;
using User = OkayegTeaTime.Database.Models.User;

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
        User? user = DbControl.Users.GetUser(chatMessage.UserId, chatMessage.Username);
        if (user is null)
        {
            DbControl.Users.Add(chatMessage.UserId, chatMessage.Username, type);
        }

        user = DbControl.Users[chatMessage.UserId];
        if (user is null)
        {
            return;
        }

        string message = chatMessage.Split[1..].JoinToString(' ');
        user.AfkMessage = message;
        user.AfkType = type;
        user.AfkTime = TimeHelper.Now();
        user.IsAfk = true;

        AfkMessage afkMessage = new(chatMessage.UserId);
        if (afkMessage.GoingAway is null)
        {
            return;
        }

        TwitchBot.Send(chatMessage.Channel, afkMessage.GoingAway);
    }
}
