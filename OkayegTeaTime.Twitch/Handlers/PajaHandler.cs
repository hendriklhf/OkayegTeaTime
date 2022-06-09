using System.Text.RegularExpressions;

namespace OkayegTeaTime.Twitch.Handlers;

public abstract class PajaHandler : Handler
{
    private protected const int _pajaAlertUserId = 82008718;
    private protected const int _pajaChannelId = 11148817;
    private protected const string _pajaAlertChannel = "pajlada";

    protected abstract Regex Pattern { get; }

    protected abstract string Message { get; }

    protected PajaHandler(TwitchBot twitchBot) : base(twitchBot)
    {
    }
}
