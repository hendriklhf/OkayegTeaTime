using System;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class AfkCommandHandler
{
    private readonly TwitchBot _twitchBot;

    public AfkCommandHandler(TwitchBot twitchBot)
    {
        _twitchBot = twitchBot;
    }

    public async ValueTask Handle(ChatMessage chatMessage, AfkType afkType)
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
        user.AfkTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        user.IsAfk = true;

        string emote = _twitchBot.Channels[chatMessage.Channel]?.Emote ?? AppSettings.DefaultEmote;
        using PoolBufferStringBuilder responseBuilder = new(AppSettings.MaxMessageLength);
        responseBuilder.Append(emote, " ");
        int afkMessageLength = _twitchBot.AfkMessageBuilder.BuildGoingAwayMessage(chatMessage.Username, afkType, responseBuilder.FreeBufferSpan);
        responseBuilder.Advance(afkMessageLength);

        await _twitchBot.SendAsync(chatMessage.Channel, responseBuilder.WrittenMemory);
    }
}
