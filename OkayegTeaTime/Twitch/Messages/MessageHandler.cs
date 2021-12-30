using System.Text.RegularExpressions;
using HLE.Emojis;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Commands;
using OkayegTeaTime.Twitch.Handlers;
using OkayegTeaTime.Twitch.Messages.Enums;
using OkayegTeaTime.Twitch.Messages.Interfaces;
using TwitchLib.Client.Models;

namespace OkayegTeaTime.Twitch.Messages;

public class MessageHandler : Handler
{
    public CommandHandler CommandHandler { get; }

    private const string _pajaAlertUsername = "pajbot";
    private static readonly Regex _pajaAlertPattern = new($@"^\s*pajaS\s+{Emoji.RotatingLight}\s+ALERT\s*$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private const string _pajaAlertChannel = "pajlada";
    private const string _pajaAlertEmote = "pajaStare";
    private const string _pajaAlertMessage = $"/me {_pajaAlertEmote} {Emoji.RotatingLight} OBACHT";

    private static readonly Regex _forgottenPrefixPattern = new($@"^@?{AppSettings.Twitch.Username},?\s(pre|suf)fix", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public MessageHandler(TwitchBot twitchBot)
        : base(twitchBot)
    {
        CommandHandler = new(twitchBot);
    }

    public override void Handle(ITwitchChatMessage chatMessage)
    {
        if (!chatMessage.UserTags.Contains(UserTag.Special))
        {
            DatabaseController.AddUser(chatMessage.Username);

            DatabaseController.CheckIfAfk(TwitchBot, chatMessage);

            DatabaseController.CheckForReminder(TwitchBot, chatMessage);

            CommandHandler.Handle(chatMessage);

            HandleSpecificMessages(chatMessage);
        }
    }

    private void HandleSpecificMessages(ITwitchChatMessage chatMessage)
    {
        CheckForSpotifyUri(chatMessage);
        CheckForForgottenPrefix(chatMessage);
    }

    public void CheckForPajaAlert(ChatMessage chatMessage)
    {
        if (chatMessage.Username == _pajaAlertUsername && _pajaAlertPattern.IsMatch(chatMessage.Message))
        {
            TwitchBot.TwitchClient.SendMessage(_pajaAlertChannel, _pajaAlertMessage);
        }
    }

    private void CheckForSpotifyUri(ITwitchChatMessage chatMessage)
    {
        if (chatMessage.Channel.Name == AppSettings.SecretOfflineChatChannel)
        {
            string uri = BotActions.SendDetectedSpotifyUri(chatMessage);
            if (!string.IsNullOrEmpty(uri))
            {
                TwitchBot.Send(AppSettings.SecretOfflineChatChannel, uri);
            }
        }
    }

    private void CheckForForgottenPrefix(ITwitchChatMessage chatMessage)
    {
        if (_forgottenPrefixPattern.IsMatch(chatMessage.Message))
        {
            if (string.IsNullOrEmpty(chatMessage.Channel.Prefix))
            {
                TwitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, Suffix: {AppSettings.Suffix}");
            }
            else
            {
                TwitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, Prefix: {chatMessage.Channel.Prefix}");
            }
        }
    }
}
