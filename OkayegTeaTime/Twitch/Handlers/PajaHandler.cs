using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public abstract class PajaHandler : Handler
{
    private const int _pajaAlertUserId = 82008718;
    private const int _pajaChannelId = 11148817;
    private const string _pajaAlertChannel = "pajlada";

    protected abstract Regex Pattern { get; }

    protected abstract string Message { get; }

    protected PajaHandler(TwitchBot twitchBot) : base(twitchBot)
    {
    }

    public override void Handle(TwitchChatMessage chatMessage)
    {
        if (chatMessage.ChannelId != _pajaChannelId || chatMessage.UserId != _pajaAlertUserId || !Pattern.IsMatch(chatMessage.Message))
        {
            return;
        }

        _twitchBot.TwitchClient.SendMessage(_pajaAlertChannel, Message);
    }
}
