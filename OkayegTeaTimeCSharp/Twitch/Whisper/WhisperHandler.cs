using OkayegTeaTimeCSharp.Twitch.Bot;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Whisper
{
    public class WhisperHandler : Handler
    {
        public WhisperMessage WhisperMessage { get; }

        public WhisperHandler(TwitchBot twitchBot, TwitchLibMessage chatMessage)
            : base(twitchBot)
        {
            WhisperMessage = chatMessage as WhisperMessage;
        }

        public override void Handle()
        {
        }
    }
}