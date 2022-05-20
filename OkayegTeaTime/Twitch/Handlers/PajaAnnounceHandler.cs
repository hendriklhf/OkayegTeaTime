using OkayegTeaTime.Twitch.Bot;

namespace OkayegTeaTime.Twitch.Handlers;

public class PajaAnnounceHandler : PajaHandler
{
    // TODO: create pattern
    protected override Regex Pattern { get; } = new("", RegexOptions.Compiled);
    protected override string Message => "/announce xd";

    // TODO: implement class
    public PajaAnnounceHandler(TwitchBot twitchBot) : base(twitchBot)
    {
    }
}
