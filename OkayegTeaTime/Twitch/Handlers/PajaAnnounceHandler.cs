using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public class PajaAnnounceHandler : PajaHandler
{
    protected override Regex Pattern { get; } = new(@"^\s*/announce n($|\s)", RegexOptions.Compiled);

    protected override string Message => " /announce o miniDank";

    public PajaAnnounceHandler(TwitchBot twitchBot) : base(twitchBot)
    {
    }

    public override void Handle(TwitchChatMessage chatMessage)
    {
        if (chatMessage.ChannelId == _pajaChannelId && Pattern.IsMatch(chatMessage.Message))
        {
            _twitchBot.SendText(_pajaAlertChannel, Message);
        }
    }
}
