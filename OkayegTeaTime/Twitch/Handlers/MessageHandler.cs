using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
#if !DEBUG
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Spotify.Exceptions;
#endif
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public class MessageHandler : Handler
{
    private readonly CommandHandler _commandHandler;
    private readonly PajaAlertHandler _pajaAlertHandler;
    private readonly PajaAnnounceHandler _pajaAnnounceHandler;

    private readonly Regex _forgottenPrefixPattern = new($@"^@?{AppSettings.Twitch.Username},?\s(pre|suf)fix", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
#if !DEBUG
    private readonly Regex _spotifyUriPattern = new(Pattern.SpotifyUri, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private readonly Regex _spotifyUrlPattern = new(Pattern.SpotifyLink, RegexOptions.IgnoreCase | RegexOptions.Compiled);
#endif

    public MessageHandler(TwitchBot twitchBot)
        : base(twitchBot)
    {
        _commandHandler = new(twitchBot);
        _pajaAlertHandler = new(twitchBot);
        _pajaAnnounceHandler = new(twitchBot);
    }

    public override void Handle(TwitchChatMessage chatMessage)
    {
        _pajaAlertHandler.Handle(chatMessage);
        _pajaAnnounceHandler.Handle(chatMessage);

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
#if !DEBUG
        CheckForSpotifyUri(chatMessage);
#endif
        CheckForForgottenPrefix(chatMessage);
    }

    private void CheckForAfk(TwitchChatMessage chatMessage)
    {
        User? user = DbControl.Users.GetUser(chatMessage.UserId, chatMessage.Username);
        if (user?.IsAfk != true)
        {
            return;
        }

        _twitchBot.SendComingBack(chatMessage);
        if (!_twitchBot.CommandController.IsAfkCommand(chatMessage))
        {
            user.IsAfk = false;
        }
    }

    private void CheckForReminder(string username, string channel)
    {
        IEnumerable<Reminder> reminders = DbControl.Reminders.GetRemindersFor(username, ReminderType.NonTimed);
        _twitchBot.SendReminder(channel, reminders);
    }

#if !DEBUG
    private void CheckForSpotifyUri(TwitchChatMessage chatMessage)
    {
        if (chatMessage.Channel != AppSettings.OfflineChatChannel)
        {
            return;
        }

        string[] songs = chatMessage.Split.Where(s => _spotifyUriPattern.IsMatch(s) || _spotifyUrlPattern.IsMatch(s)).Select(s => SpotifyController.ParseSongToUri(s)!).ToArray();
        SpotifyUser? playlistUser = DbControl.SpotifyUsers["strbhlfe"];
        if (playlistUser is null)
        {
            return;
        }

        try
        {
            playlistUser.AddToChatPlaylist(songs).Wait();
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

        string? prefix = DbControl.Channels[chatMessage.Channel]?.Prefix;
        if (string.IsNullOrEmpty(prefix))
        {
            _twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, Suffix: {AppSettings.Suffix}");
        }
        else
        {
            _twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, Prefix: {prefix}");
        }
    }
}
