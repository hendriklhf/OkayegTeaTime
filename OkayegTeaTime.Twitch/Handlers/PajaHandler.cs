using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Twitch.Handlers;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class PajaHandler(TwitchBot twitchBot) : Handler(twitchBot)
{
    private protected const int PajaAlertUserId = 82008718;
    private protected const int PajaChannelId = 11148817;
    private protected const string PajaAlertChannel = "pajlada";

    protected abstract Regex Pattern { get; }

    protected abstract string Message { get; }
}
