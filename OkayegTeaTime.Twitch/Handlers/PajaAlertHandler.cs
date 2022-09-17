using System.Text.RegularExpressions;
using HLE.Emojis;
using OkayegTeaTime.Files;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public sealed class PajaAlertHandler : PajaHandler
{
    protected override Regex Pattern { get; } = new($@"^\s*pajaS\s+{Emoji.RotatingLight}\s+ALERT\s*$", RegexOptions.Compiled);

    protected override string Message => $"/me pajaStare {Emoji.RotatingLight} OBACHT";

    public PajaAlertHandler(TwitchBot twitchBot) : base(twitchBot)
    {
    }

    public override void Handle(TwitchChatMessage chatMessage)
    {
        if (chatMessage.ChannelId != _pajaChannelId || chatMessage.UserId != _pajaAlertUserId || !Pattern.IsMatch(chatMessage.Message))
        {
            return;
        }

        _twitchBot.SendText(_pajaAlertChannel, Message);
        _twitchBot.SendText(AppSettings.OfflineChatChannel, $"{AppSettings.DefaultEmote} {Emoji.RotatingLight}");
    }
}
