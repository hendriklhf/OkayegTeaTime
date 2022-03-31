using HLE.Strings;
using TwitchLib.Client.Models;

namespace OkayegTeaTime.Twitch.Models;

public class ChatMessage
{
    public string DisplayName { get; }

    public string[] LowerSplit { get; }

    public string Message { get; }

    public string[] Split { get; }

    public string Username { get; }

    private static readonly Regex _messagePattern = new(@"(WHISPER|PRIVMSG)\s#?\w+\s:.+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _messageReplacePattern = new(@"^(WHISPER|PRIVMSG)\s#?\w+\s:", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public ChatMessage(TwitchLibMessage twitchLibMessage)
    {
        DisplayName = twitchLibMessage.DisplayName;
        Message = GetMessage(twitchLibMessage).RemoveChatterinoChar().TrimAll();
        LowerSplit = GetLowerSplit();
        Split = GetSplit();
        Username = twitchLibMessage.Username;
    }

    private string GetMessage(TwitchLibMessage twitchLibMessage)
    {
        string message = _messagePattern.Match(twitchLibMessage.RawIrcMessage).Value;
        return _messageReplacePattern.Replace(message, "");
    }

    private string[] GetSplit()
    {
        return Message.Split();
    }

    private string[] GetLowerSplit()
    {
        return Message.ToLower().Split();
    }
}
