using System.Text.RegularExpressions;
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
    public CommandHandler CommandHandler { get; }

    public Throttler Throttler { get; }

    private readonly LinkRecognizer _linkRecognizer = new();

    private static readonly Regex _pajaAlertPattern = new($@"^\s*pajaS\s+{Emoji.RotatingLight}\s+ALERT\s*$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _forgottenPrefixPattern = new($@"^@?{AppSettings.Twitch.Username},?\s(pre|suf)fix", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    private const int _pajaAlertUserId = 82008718;
    private const int _pajaChannelId = 11148817;
    private const string _pajaAlertChannel = "pajlada";
    private const string _pajaAlertEmote = "pajaStare";
    private const string _pajaAlertMessage = $"/me {_pajaAlertEmote} {Emoji.RotatingLight} OBACHT";

    public MessageHandler(TwitchBot twitchBot)
        : base(twitchBot)
    {
        CommandHandler = new(twitchBot);
        Throttler = new(twitchBot);
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
            TwitchBot.SendComingBack(chatMessage);
            if (!chatMessage.IsAfkCommmand)
            {
                user.IsAfk = false;
            }
        }

        DbController.CheckForReminder(TwitchBot, chatMessage);

        //if (!Throttler.CanBeProcessed(chatMessage))
        //{
        //    return;
        //}

        CommandHandler.Handle(chatMessage);

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
            TwitchBot.TwitchClient.SendMessage(_pajaAlertChannel, _pajaAlertMessage);
            TwitchBot.TwitchClient.SendMessage(AppSettings.SecretOfflineChatChannel, $"{AppSettings.DefaultEmote} {Emoji.RotatingLight}");
        }
    }

    private void CheckForSpotifyUri(TwitchChatMessage chatMessage)
    {
        if (chatMessage.Channel == AppSettings.SecretOfflineChatChannel)
        {
            string? uri = _linkRecognizer.FindSpotifyLink(chatMessage);
            if (!string.IsNullOrEmpty(uri))
            {
                TwitchBot.Send(AppSettings.SecretOfflineChatChannel, uri);
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
                TwitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, Suffix: {AppSettings.Suffix}");
            }
            else
            {
                TwitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, Prefix: {prefix}");
            }
        }
    }
}
