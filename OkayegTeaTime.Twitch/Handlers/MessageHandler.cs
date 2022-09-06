using System;
using System.Collections.Generic;
#if RELEASE
using System.Linq;
#endif
using System.Text.RegularExpressions;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files;
#if RELEASE
using OkayegTeaTime.Spotify;
#endif
using OkayegTeaTime.Twitch.Models;
#if RELEASE
using OkayegTeaTime.Utils;
#endif

namespace OkayegTeaTime.Twitch.Handlers;

public class MessageHandler : Handler
{
    private readonly CommandHandler _commandHandler;
    private readonly PajaAlertHandler _pajaAlertHandler;

    private readonly Regex _forgottenPrefixPattern = new($@"^@?{AppSettings.Twitch.Username},?\s(pre|suf)fix", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public MessageHandler(TwitchBot twitchBot)
        : base(twitchBot)
    {
        _commandHandler = new(twitchBot);
        _pajaAlertHandler = new(twitchBot);
    }

    public override void Handle(TwitchChatMessage chatMessage)
    {
        _pajaAlertHandler.Handle(chatMessage);

        if (chatMessage.IsIgnoredUser)
        {
            return;
        }

        CheckForAfk(chatMessage);

        CheckForReminder(chatMessage.Username, chatMessage.Channel);

        _commandHandler.Handle(chatMessage);

        HandleSpecificMessages(chatMessage);
    }

    private void HandleSpecificMessages(TwitchChatMessage chatMessage)
    {
#if RELEASE
        CheckForSpotifyUri(chatMessage);
#endif
        CheckForForgottenPrefix(chatMessage);
    }

    private void CheckForAfk(TwitchChatMessage chatMessage)
    {
        User? user = _twitchBot.Users.GetUser(chatMessage.UserId, chatMessage.Username);
        if (user?.IsAfk != true)
        {
            return;
        }

        _twitchBot.SendComingBack(chatMessage.UserId, chatMessage.Channel);
        if (!_twitchBot.CommandController.IsAfkCommand(_twitchBot.Channels[chatMessage.ChannelId]?.Prefix, chatMessage.Message))
        {
            user.IsAfk = false;
        }
    }

    private void CheckForReminder(string username, string channel)
    {
        IEnumerable<Reminder> reminders = _twitchBot.Reminders.GetRemindersFor(username, ReminderType.NonTimed);
        _twitchBot.SendReminder(channel, reminders);
    }

#if RELEASE
    private void CheckForSpotifyUri(TwitchChatMessage chatMessage)
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

        string[] songs = chatMessage.Split.Where(s => Pattern.SpotifyLink.IsMatch(s) || Pattern.SpotifyUri.IsMatch(s)).Select(s => SpotifyController.ParseSongToUri(s)!).ToArray();
        try
        {
            playlistUser.AddToChatPlaylist(songs);
        }
        catch (SpotifyException)
        {
            // ignored
        }
    }
#endif

    private void CheckForForgottenPrefix(TwitchChatMessage chatMessage)
    {
        if (!_forgottenPrefixPattern.IsMatch(chatMessage.Message))
        {
            return;
        }

        string? prefix = _twitchBot.Channels[chatMessage.Channel]?.Prefix;
        string message = string.IsNullOrEmpty(prefix)
            ? $"{chatMessage.Username}, Suffix: {AppSettings.Suffix}"
            : $"{chatMessage.Username}, Prefix: {prefix}";
        _twitchBot.Send(chatMessage.Channel, message);
    }
}
