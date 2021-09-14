using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Bot;

namespace OkayegTeaTimeCSharp.Twitch.Whisper
{
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
}
