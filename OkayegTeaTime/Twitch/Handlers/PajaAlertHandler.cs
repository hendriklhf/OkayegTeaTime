using HLE.Emojis;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public class PajaAlertHandler : Handler
{
    private readonly Regex _pajaAlertPattern = new($@"^\s*pajaS\s+{Emoji.RotatingLight}\s+ALERT\s*$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    private const int _pajaAlertUserId = 82008718;
    private const int _pajaChannelId = 11148817;
    private const string _pajaAlertChannel = "pajlada";
    private const string _pajaAlertEmote = "pajaStare";
    private const string _pajaAlertMessage = $"/me {_pajaAlertEmote} {Emoji.RotatingLight} OBACHT";

    public PajaAlertHandler(TwitchBot twitchBot) : base(twitchBot)
    {
    }

    public override void Handle(TwitchChatMessage chatMessage)
    {
        if (chatMessage.ChannelId != _pajaChannelId || chatMessage.UserId != _pajaAlertUserId || !_pajaAlertPattern.IsMatch(chatMessage.Message))
        {
            return;
        }

        _twitchBot.Send(_pajaAlertChannel, _pajaAlertMessage);
        _twitchBot.Send(AppSettings.OfflineChatChannel, $"{AppSettings.DefaultEmote} {Emoji.RotatingLight}");
    }
}
