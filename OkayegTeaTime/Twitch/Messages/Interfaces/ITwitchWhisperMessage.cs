namespace OkayegTeaTime.Twitch.Messages.Interfaces;

public interface ITwitchWhisperMessage : ITwitchMessage
{
    public int MessageId { get; }

    public int ThreadId { get; }
}
