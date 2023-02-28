using System;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class AfkCommandHandler
{
    private readonly TwitchBot _twitchBot;

    public AfkCommandHandler(TwitchBot twitchBot)
    {
        _twitchBot = twitchBot;
    }

    public void Handle(ChatMessage chatMessage, AfkType type)
    {
        User? user = _twitchBot.Users.Get(chatMessage.UserId, chatMessage.Username);
        if (user is null)
        {
            user = new(chatMessage.UserId, chatMessage.Username);
            _twitchBot.Users.Add(user);
        }

        using ChatMessageExtension messageExtension = new(chatMessage);
        string? message = messageExtension.Split.Length > 1 ? chatMessage.Message[(messageExtension.Split[0].Length + 1)..] : null;
        user.AfkMessage = message;
        user.AfkType = type;
        user.AfkTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        user.IsAfk = true;

        AfkCommand cmd = _twitchBot.CommandController[type];
        AfkMessage afkMessage = new(user, cmd);

        _twitchBot.Send(chatMessage.Channel, afkMessage.GoingAway);
    }
}
