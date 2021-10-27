namespace OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

public interface IChatMessage
{
    public string DisplayName { get; }

    public string[] LowerSplit { get; }

    public string Message { get; }

    public string[] Split { get; }

    public string Username { get; }
}
