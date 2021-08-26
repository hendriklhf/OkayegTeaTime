using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Whisper
{
    public class WhisperHandler : Handler
    {
        public WhisperMessage WhisperMessage { get; }

        public WhisperHandler(TwitchBot twitchBot, WhisperMessage whisperMessage)
            : base(twitchBot)
        {
            WhisperMessage = whisperMessage;
        }

        public override void Handle()
        {
        }
    }
}
