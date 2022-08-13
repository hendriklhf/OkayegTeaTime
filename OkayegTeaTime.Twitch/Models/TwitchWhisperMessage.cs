using TwitchLib.Client.Models;

namespace OkayegTeaTime.Twitch.Models;

public class TwitchWhisperMessage : TwitchMessage
{
    public int MessageId { get; }

    public string ThreadId { get; }

    public TwitchWhisperMessage(WhisperMessage whisperMessage)
        : base(whisperMessage)
    {
        MessageId = int.Parse(whisperMessage.MessageId);
        ThreadId = whisperMessage.ThreadId;
    }
}
