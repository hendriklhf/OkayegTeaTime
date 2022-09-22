using System;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class AfkCommandHandler
{
    private readonly TwitchBot _twitchBot;

    public AfkCommandHandler(TwitchBot twitchBot)
    {
        _twitchBot = twitchBot;
    }

    public void Handle(TwitchChatMessage chatMessage, AfkType type)
    {
        User? user = _twitchBot.Users.GetUser(chatMessage.UserId, chatMessage.Username);
        if (user is null)
        {
            user = new(chatMessage.UserId, chatMessage.Username);
            _twitchBot.Users.Add(user);
        }

        string? message = chatMessage.Split.Length > 1 ? chatMessage.Message[(chatMessage.Split[0].Length + 1)..] : null;
        user.AfkMessage = message;
        user.AfkType = type;
        user.AfkTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        user.IsAfk = true;

        AfkCommand cmd = _twitchBot.CommandController[type];
        AfkMessage afkMessage = new(user, cmd);

        _twitchBot.Send(chatMessage.Channel, afkMessage.GoingAway);
    }
}
