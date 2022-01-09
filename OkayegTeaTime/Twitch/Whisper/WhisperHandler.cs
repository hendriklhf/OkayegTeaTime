using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Whisper;

public class WhisperHandler
{
    public TwitchBot TwitchBot { get; }

    public WhisperHandler(TwitchBot twitchBot)
    {
        TwitchBot = twitchBot;
    }

    public void Handle(TwitchWhisperMessage whisperMessage)
    {
    }
}
