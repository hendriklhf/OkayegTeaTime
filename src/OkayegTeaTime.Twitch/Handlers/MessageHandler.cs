using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;
#if RELEASE
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using HLE.Marshalling;
using OkayegTeaTime.Database;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Utils;
#endif

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class MessageHandler(TwitchBot twitchBot) : Handler(twitchBot)
{
    private readonly CommandHandler _commandHandler = new(twitchBot);
    private readonly PajaAlertHandler _pajaAlertHandler = new(twitchBot);

    private static readonly Regex s_forgottenPrefixPattern = new($@"^@?{GlobalSettings.Settings.Twitch.Username},?\s*(pre|suf)fix", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public override async ValueTask HandleAsync(IChatMessage chatMessage)
    {
        await _pajaAlertHandler.HandleAsync(chatMessage);

        using ChatMessageExtension messageExtension = new(chatMessage);
        if (messageExtension.IsIgnoredUser)
        {
            return;
        }

        await CheckForAfkAsync(chatMessage);
        await CheckForReminderAsync(chatMessage.Username, chatMessage.Channel);
        await _commandHandler.HandleAsync(chatMessage);
        await HandleSpecificMessagesAsync(chatMessage);
    }

    // ReSharper disable once ReplaceAsyncWithTaskReturn
    private async ValueTask HandleSpecificMessagesAsync(IChatMessage chatMessage)
    {
#if RELEASE
        await CheckForSpotifyUriAsync(chatMessage);
#endif
        await CheckForForgottenPrefixAsync(chatMessage);
    }

    private async ValueTask CheckForAfkAsync(IChatMessage chatMessage)
    {
        User? user = _twitchBot.Users.Get(chatMessage.UserId, chatMessage.Username);
        if (user?.IsAfk != true)
        {
            return;
        }

        await _twitchBot.SendComingBackAsync(user, chatMessage.Channel);
        string? channelPrefix = _twitchBot.Channels[chatMessage.ChannelId]?.Prefix;
        if (!CommandController.IsAfkCommand(channelPrefix, chatMessage.Message) || _twitchBot.CooldownController.IsOnAfkCooldown(chatMessage.UserId))
        {
            user.IsAfk = false;
        }
    }

    // ReSharper disable once InconsistentNaming
    private ValueTask CheckForReminderAsync(string username, string channel)
    {
        Reminder[] reminders = _twitchBot.Reminders.GetReminders(username, ReminderTypes.NonTimed);
        return _twitchBot.SendReminderAsync(channel, reminders);
    }

#if RELEASE
    private async ValueTask CheckForSpotifyUriAsync(IChatMessage chatMessage)
    {
        if (GlobalSettings.Settings.OfflineChat is null || GlobalSettings.Settings.Spotify is null)
        {
            // settings are not configured, return
            return;
        }

        if (chatMessage.Channel != GlobalSettings.Settings.OfflineChat.Channel)
        {
            return;
        }

        string? username = _twitchBot.Users[GlobalSettings.Settings.Users.Owner]?.Username;
        if (username is null)
        {
            return;
        }

        SpotifyUser? playlistUser = _twitchBot.SpotifyUsers[username];
        if (playlistUser is null)
        {
            return;
        }

        using ChatMessageExtension messageExtension = new(chatMessage);
        IEnumerable<ReadOnlyMemory<char>> splits = MemoryMarshal.ToEnumerable(SpanMarshal.AsMemory(messageExtension.Split.AsSpan()));
        string[] songs = splits
            .Where(static s => Pattern.SpotifyLink.IsMatch(s.Span) || Pattern.SpotifyUri.IsMatch(s.Span))
            .Select(static s => SpotifyController.ParseSongToUri(s.Span)!).ToArray();

        try
        {
            await SpotifyController.AddToPlaylistAsync(playlistUser, songs);
        }
        catch (SpotifyException ex)
        {
            await DbController.LogExceptionAsync(ex);
        }
    }
#endif

    // ReSharper disable once InconsistentNaming
    private ValueTask CheckForForgottenPrefixAsync(IChatMessage chatMessage)
    {
        if (!s_forgottenPrefixPattern.IsMatch(chatMessage.Message))
        {
            return ValueTask.CompletedTask;
        }

        string? prefix = _twitchBot.Channels[chatMessage.Channel]?.Prefix;
        string message = string.IsNullOrWhiteSpace(prefix) ? $"{chatMessage.Username}, Suffix: {GlobalSettings.Suffix}" : $"{chatMessage.Username}, Prefix: {prefix}";
        return _twitchBot.SendAsync(chatMessage.Channel, message);
    }
}
