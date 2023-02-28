using System;
#if RELEASE
using System.Linq;
using HLE.Memory;
using OkayegTeaTime.Database;
#endif
using System.Text.RegularExpressions;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Models;
#if RELEASE
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Utils;
#endif

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class MessageHandler : Handler
{
    private readonly CommandHandler _commandHandler;
    private readonly PajaAlertHandler _pajaAlertHandler;

    private readonly Regex _forgottenPrefixPattern = new($@"^@?{AppSettings.Twitch.Username},?\s(pre|suf)fix", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public MessageHandler(TwitchBot twitchBot) : base(twitchBot)
    {
        _commandHandler = new(twitchBot);
        _pajaAlertHandler = new(twitchBot);
    }

    public override void Handle(ChatMessage chatMessage)
    {
        _pajaAlertHandler.Handle(chatMessage);

        using ChatMessageExtension messageExtension = new(chatMessage);
        if (messageExtension.IsIgnoredUser)
        {
            return;
        }

        CheckForAfk(chatMessage);

        CheckForReminder(chatMessage.Username, chatMessage.Channel);

        _commandHandler.Handle(chatMessage);

        HandleSpecificMessages(chatMessage);
    }

    private void HandleSpecificMessages(ChatMessage chatMessage)
    {
#if RELEASE
        CheckForSpotifyUri(chatMessage);
#endif
        CheckForForgottenPrefix(chatMessage);
    }

    private void CheckForAfk(ChatMessage chatMessage)
    {
        User? user = _twitchBot.Users.Get(chatMessage.UserId, chatMessage.Username);
        if (user?.IsAfk != true)
        {
            return;
        }

        _twitchBot.SendComingBack(user, chatMessage.Channel);
        if (!_twitchBot.CommandController.IsAfkCommand(_twitchBot.Channels[chatMessage.ChannelId]?.Prefix, chatMessage.Message))
        {
            user.IsAfk = false;
        }
    }

    private void CheckForReminder(string username, string channel)
    {
        Reminder[] reminders = _twitchBot.Reminders.GetRemindersFor(username, ReminderType.NonTimed);
        _twitchBot.SendReminder(channel, reminders);
    }

#if RELEASE
    private void CheckForSpotifyUri(ChatMessage chatMessage)
    {
        if (chatMessage.Channel != AppSettings.OfflineChatChannel)
        {
            return;
        }

        string? username = _twitchBot.Users[AppSettings.UserLists.Owner]?.Username;
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
        using RentedArray<ReadOnlyMemory<char>> splits = messageExtension.Split.GetSplits();
        string[] songs = splits.Where(s => Pattern.SpotifyLink.IsMatch(s.Span) || Pattern.SpotifyUri.IsMatch(s.Span)).Select(s => SpotifyController.ParseSongToUri(s.Span)!).ToArray();
        try
        {
            SpotifyController.AddToPlaylist(playlistUser, songs);
        }
        catch (SpotifyException ex)
        {
            DbController.LogException(ex);
        }
    }
#endif

    private void CheckForForgottenPrefix(ChatMessage chatMessage)
    {
        if (!_forgottenPrefixPattern.IsMatch(chatMessage.Message))
        {
            return;
        }

        string? prefix = _twitchBot.Channels[chatMessage.Channel]?.Prefix;
        string message = string.IsNullOrWhiteSpace(prefix) ? $"{chatMessage.Username}, Suffix: {AppSettings.Suffix}" : $"{chatMessage.Username}, Prefix: {prefix}";
        _twitchBot.Send(chatMessage.Channel, message);
    }
}
