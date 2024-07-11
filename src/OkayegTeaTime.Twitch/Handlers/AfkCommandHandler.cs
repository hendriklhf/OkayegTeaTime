using System;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class AfkCommandHandler(TwitchBot twitchBot)
{
    private readonly TwitchBot _twitchBot = twitchBot;

    public async ValueTask HandleAsync(IChatMessage chatMessage, AfkType afkType)
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
        user.AfkType = afkType;
#pragma warning disable S6354
        user.AfkTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
#pragma warning restore S6354
        user.IsAfk = true;

        string emote = _twitchBot.Channels[chatMessage.Channel]?.Emote ?? GlobalSettings.DefaultEmote;
        using PooledStringBuilder responseBuilder = new(GlobalSettings.MaxMessageLength);
        responseBuilder.Append(emote);
        responseBuilder.Append(' ');
        int afkMessageLength = _twitchBot.AfkMessageBuilder.BuildGoingAwayMessage(chatMessage.Username, afkType, responseBuilder.FreeBufferSpan);
        responseBuilder.Advance(afkMessageLength);

        await _twitchBot.SendAsync(chatMessage.Channel, responseBuilder.WrittenMemory);
    }
}
