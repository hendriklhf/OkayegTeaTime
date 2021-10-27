using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Whisper;

public class WhisperHandler
{
    public TwitchBot TwitchBot { get; }

    public WhisperHandler(TwitchBot twitchBot)
    {
        TwitchBot = twitchBot;
    }

    public void Handle(ITwitchWhisperMessage whisperMessage)
    {
    }
}
