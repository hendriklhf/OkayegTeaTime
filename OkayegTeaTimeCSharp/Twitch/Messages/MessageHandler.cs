using System.Text.RegularExpressions;
using HLE.Emojis;
using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Commands;
using OkayegTeaTimeCSharp.Twitch.Handlers;
using OkayegTeaTimeCSharp.Twitch.Messages.Enums;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Messages;

public class MessageHandler : Handler
{
    public CommandHandler CommandHandler { get; }

    private const string _pajaAlertUsername = "pajbot";
    private static readonly Regex _pajaAlertPattern = new($@"^\s*pajaS\s+{Emoji.RotatingLight}\s+ALERT\s*$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private const string _pajaAlertChannel = "pajlada";
    private const string _pajaAlertEmote = "pajaStare";
    private const string _pajaAlertMessage = $"/me {_pajaAlertEmote} {Emoji.RotatingLight} OBACHT";

    private static readonly Regex _forgottenPrefixPattern = new($@"^@?{Settings.Username},?\s(pre|suf)fix(\s|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

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

            DatabaseController.AddMessage(chatMessage);

            DatabaseController.CheckIfAFK(TwitchBot, chatMessage);

            DatabaseController.CheckForReminder(TwitchBot, chatMessage);

            CommandHandler.Handle(chatMessage);

            DatabaseController.CheckForNukes(TwitchBot, chatMessage);

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
        if (chatMessage.Channel.Name == Settings.SecretOfflineChat)
        {
            string uri = BotActions.SendDetectedSpotifyURI(chatMessage);
            if (!string.IsNullOrEmpty(uri))
            {
                TwitchBot.Send(Settings.SecretOfflineChat, uri);
            }
        }
    }

    private void CheckForForgottenPrefix(ITwitchChatMessage chatMessage)
    {
        if (_forgottenPrefixPattern.IsMatch(chatMessage.Message))
        {
            if (string.IsNullOrEmpty(chatMessage.Channel.Prefix))
            {
                TwitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, Suffix: {Config.Suffix}");
            }
            else
            {
                TwitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, Prefix: {chatMessage.Channel.Prefix}");
            }
        }
    }
}
