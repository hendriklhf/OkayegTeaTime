using System;
using System.Threading.Tasks;
using HLE.Text;
using HLE.Twitch.Tmi.Models;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class AfkCommandHandler(TwitchBot twitchBot)
{
    private readonly TwitchBot _twitchBot = twitchBot;

    public async ValueTask HandleAsync(ChatMessage chatMessage, AfkType afkType)
    {
        User? user = _twitchBot.Users.Get(chatMessage.UserId, chatMessage.Username.ToString());
        if (user is null)
        {
            user = new(chatMessage.UserId, chatMessage.Username.ToString());
            _twitchBot.Users.Add(user);
        }

        using ChatMessageExtension messageExtension = new(chatMessage);
        // TODO: fix ToString call
        string? message = messageExtension.Split.Length > 1 ? chatMessage.Message.ToString()[(messageExtension.Split[0].Length + 1)..] : null;
        user.AfkMessage = message;
        user.AfkType = afkType;
#pragma warning disable S6354
        user.AfkTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
#pragma warning restore S6354
        user.IsAfk = true;

        string emote = _twitchBot.Channels[chatMessage.Channel]?.Emote ?? GlobalSettings.DefaultEmote;
        using PooledStringBuilder responseBuilder = new(GlobalSettings.MaxMessageLength);
        responseBuilder.Append(emote);
        responseBuilder.Append(' ');
        _twitchBot.AfkMessageBuilder.BuildGoingAwayMessage(chatMessage.Username.AsSpan(), afkType, responseBuilder);

        await _twitchBot.SendAsync(chatMessage.Channel, responseBuilder.WrittenMemory);
    }
}
