using HLE.Emojis;
using HLE.Strings;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Spotify;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;
using TwitchLib = TwitchLib.Client.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public class MessageHandler : Handler
{
    private readonly CommandHandler _commandHandler;

    private readonly LinkRecognizer _linkRecognizer = new();

    private readonly Regex _pajaAlertPattern = new($@"^\s*pajaS\s+{Emoji.RotatingLight}\s+ALERT\s*$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private readonly Regex _forgottenPrefixPattern = new($@"^@?{AppSettings.Twitch.Username},?\s(pre|suf)fix", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    private const int _pajaAlertUserId = 82008718;
    private const int _pajaChannelId = 11148817;
    private const string _pajaAlertChannel = "pajlada";
    private const string _pajaAlertEmote = "pajaStare";
    private const string _pajaAlertMessage = $"/me {_pajaAlertEmote} {Emoji.RotatingLight} OBACHT";

    public MessageHandler(TwitchBot twitchBot)
        : base(twitchBot)
    {
        _commandHandler = new(twitchBot);
    }

    public override void Handle(TwitchChatMessage chatMessage)
    {
        if (chatMessage.IsIgnoredUser)
        {
            return;
        }

        User? user = DbControl.Users.GetUser(chatMessage.UserId, chatMessage.Username);
        if (user?.IsAfk == true)
        {
            _twitchBot.SendComingBack(chatMessage);
            if (!chatMessage.IsAfkCommmand)
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

    public void CheckForPajaAlert(TwitchLib::ChatMessage chatMessage)
    {
        if (chatMessage.RoomId.ToInt() == _pajaChannelId && chatMessage.UserId.ToInt() == _pajaAlertUserId && _pajaAlertPattern.IsMatch(chatMessage.Message))
        {
            _twitchBot.TwitchClient.SendMessage(_pajaAlertChannel, _pajaAlertMessage);
            _twitchBot.TwitchClient.SendMessage(AppSettings.SecretOfflineChatChannel, $"{AppSettings.DefaultEmote} {Emoji.RotatingLight}");
        }
    }

    private void CheckForSpotifyUri(TwitchChatMessage chatMessage)
    {
        if (chatMessage.Channel == AppSettings.SecretOfflineChatChannel)
        {
            string? uri = _linkRecognizer.FindSpotifyLink(chatMessage);
            if (!string.IsNullOrEmpty(uri))
            {
                _twitchBot.Send(AppSettings.SecretOfflineChatChannel, uri);
            }
        }
    }

    private void CheckForForgottenPrefix(TwitchChatMessage chatMessage)
    {
        if (_forgottenPrefixPattern.IsMatch(chatMessage.Message))
        {
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
}
