using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Spotify.Exceptions;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public class MessageHandler : Handler
{
    private readonly CommandHandler _commandHandler;
    private readonly PajaAlertHandler _pajaAlertHandler;

    private readonly Regex _forgottenPrefixPattern = new($@"^@?{AppSettings.Twitch.Username},?\s(pre|suf)fix", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private readonly Regex _spotifyUriPattern = new(Pattern.SpotifyUri, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private readonly Regex _spotifyUrlPattern = new(Pattern.SpotifyLink, RegexOptions.IgnoreCase | RegexOptions.Compiled);

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

        User? user = DbControl.Users.GetUser(chatMessage.UserId, chatMessage.Username);
        if (user?.IsAfk == true)
        {
            _twitchBot.SendComingBack(chatMessage);
            if (!_twitchBot.CommandController.IsAfkCommand(chatMessage))
            {
                user.IsAfk = false;
            }
        }

        DbController.CheckForReminder(_twitchBot, chatMessage);

        _commandHandler.Handle(chatMessage);

        HandleSpecificMessages(chatMessage);
    }

    private void HandleSpecificMessages(TwitchChatMessage chatMessage)
    {
        CheckForSpotifyUri(chatMessage);
        CheckForForgottenPrefix(chatMessage);
    }

    private void CheckForSpotifyUri(TwitchChatMessage chatMessage)
    {
        if (chatMessage.Channel != AppSettings.OfflineChatChannel)
        {
            return;
        }

        string[] songs = chatMessage.Split.Where(s => _spotifyUriPattern.IsMatch(s) || _spotifyUrlPattern.IsMatch(s)).Select(s => SpotifyController.ParseSongToUri(s) ?? string.Empty).ToArray();
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
