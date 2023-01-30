using System;
using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Files;
using TwitchLib.Client.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace OkayegTeaTime.Twitch.Models;

public class ChatMessage
{
    public string DisplayName { get; }

    public string[] LowerSplit => _lowerSplit ??= GetLowerSplit();

    public string Message { get; }

    public string[] Split => _split ??= GetSplit();

    public string Username { get; }

    private string[]? _lowerSplit;
    private string[]? _split;

    private static readonly Regex _messagePattern = new(@"(WHISPER|PRIVMSG)\s#?\w+\s:.+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly Regex _messageReplacePattern = new(@"^(WHISPER|PRIVMSG)\s#?\w+\s:", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    protected ChatMessage(TwitchLibMessage message)
    {
        DisplayName = message.DisplayName;
        Message = GetMessage(message).Replace(AppSettings.ChatterinoChar, string.Empty).TrimAll();
        Username = message.Username;
    }

    private static string GetMessage(TwitchLibMessage twitchLibMessage)
    {
        string message = _messagePattern.Match(twitchLibMessage.RawIrcMessage).Value;
        return _messageReplacePattern.Replace(message, string.Empty);
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
