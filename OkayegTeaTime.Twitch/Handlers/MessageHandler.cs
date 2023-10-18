using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Models;
#if RELEASE
using System.Linq;
using OkayegTeaTime.Database;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Utils;
#endif

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class MessageHandler(TwitchBot twitchBot) : Handler(twitchBot)
{
    private readonly CommandHandler _commandHandler = new(twitchBot);
    private readonly PajaAlertHandler _pajaAlertHandler = new(twitchBot);

    private readonly Regex _forgottenPrefixPattern = new($@"^@?{AppSettings.Twitch.Username},?\s*(pre|suf)fix", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public override async ValueTask Handle(IChatMessage chatMessage)
    {
        await _pajaAlertHandler.Handle(chatMessage);

        using ChatMessageExtension messageExtension = new(chatMessage);
        if (messageExtension.IsIgnoredUser)
        {
            return;
        }

        await CheckForAfkAsync(chatMessage);
        await CheckForReminderAsync(chatMessage.Username, chatMessage.Channel);
        await _commandHandler.Handle(chatMessage);
        await HandleSpecificMessagesAsync(chatMessage);
    }

    private async ValueTask HandleSpecificMessagesAsync(IChatMessage chatMessage)
    {
#if RELEASE
        await CheckForSpotifyUri(chatMessage);
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
        if (!_twitchBot.CommandController.IsAfkCommand(_twitchBot.Channels[chatMessage.ChannelId]?.Prefix, chatMessage.Message) || _twitchBot.CooldownController.IsOnAfkCooldown(chatMessage.UserId))
        {
            user.IsAfk = false;
        }
    }

    private async ValueTask CheckForReminderAsync(string username, string channel)
    {
        Reminder[] reminders = _twitchBot.Reminders.GetRemindersFor(username, ReminderType.NonTimed);
        await _twitchBot.SendReminderAsync(channel, reminders);
    }

#if RELEASE
    private async ValueTask CheckForSpotifyUri(IChatMessage chatMessage)
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
        // TODO: fix array allocation
        ReadOnlyMemory<char>[] splits = messageExtension.Split.Splits.ToArray();
        string[] songs = splits.Where(static s => Pattern.SpotifyLink.IsMatch(s.Span) || Pattern.SpotifyUri.IsMatch(s.Span)).Select(s => SpotifyController.ParseSongToUri(s.Span)!).ToArray();

        try
        {
            await SpotifyController.AddToPlaylist(playlistUser, songs);
        }
        catch (SpotifyException ex)
        {
            await DbController.LogExceptionAsync(ex);
        }
    }
#endif

    private async ValueTask CheckForForgottenPrefixAsync(IChatMessage chatMessage)
    {
        if (!_forgottenPrefixPattern.IsMatch(chatMessage.Message))
        {
            return;
        }

        string? prefix = _twitchBot.Channels[chatMessage.Channel]?.Prefix;
        string message = string.IsNullOrWhiteSpace(prefix) ? $"{chatMessage.Username}, Suffix: {AppSettings.Suffix}" : $"{chatMessage.Username}, Prefix: {prefix}";
        await _twitchBot.SendAsync(chatMessage.Channel, message);
    }
}
