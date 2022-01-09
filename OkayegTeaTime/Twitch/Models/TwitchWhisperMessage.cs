using HLE.Strings;
using TwitchLib.Client.Models;

namespace OkayegTeaTime.Twitch.Models;

public class TwitchWhisperMessage : TwitchMessage
{
    public int MessageId { get; }

    public int ThreadId { get; }

    public TwitchWhisperMessage(WhisperMessage whisperMessage)
        : base(whisperMessage)
    {
        MessageId = whisperMessage.MessageId.ToInt();
        ThreadId = whisperMessage.ThreadId.ToInt();
    }
}
