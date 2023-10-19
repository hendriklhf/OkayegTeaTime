using System;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class AfkCommandHandler(TwitchBot twitchBot)
{
    public async ValueTask Handle(IChatMessage chatMessage, AfkType afkType)
    {
        User? user = twitchBot.Users.Get(chatMessage.UserId, chatMessage.Username);
        if (user is null)
        {
            user = new(chatMessage.UserId, chatMessage.Username);
            twitchBot.Users.Add(user);
        }

        using ChatMessageExtension messageExtension = new(chatMessage);
        string? message = messageExtension.Split.Length > 1 ? chatMessage.Message[(messageExtension.Split[0].Length + 1)..] : null;
        user.AfkMessage = message;
        user.AfkType = afkType;
        user.AfkTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        user.IsAfk = true;

        string emote = twitchBot.Channels[chatMessage.Channel]?.Emote ?? GlobalSettings.DefaultEmote;
        using PooledStringBuilder responseBuilder = new(GlobalSettings.MaxMessageLength);
        responseBuilder.Append(emote, " ");
        int afkMessageLength = twitchBot.AfkMessageBuilder.BuildGoingAwayMessage(chatMessage.Username, afkType, responseBuilder.FreeBufferSpan);
        responseBuilder.Advance(afkMessageLength);

        await twitchBot.SendAsync(chatMessage.Channel, responseBuilder.WrittenMemory);
    }
}
