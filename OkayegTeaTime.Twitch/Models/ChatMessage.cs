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

    public string[] LowerSplit
    {
        get
        {
            if (_lowerSplit is not null)
            {
                return _lowerSplit;
            }

            _lowerSplit = GetLowerSplit();
            return _lowerSplit;
        }
    }

    public string Message { get; }

    public string[] Split
    {
        get
        {
            if (_split is not null)
            {
                return _split;
            }

            _split = GetSplit();
            return _split;
        }
    }

    public string Username { get; }

    private string[]? _lowerSplit;
    private string[]? _split;

    private static readonly Regex _messagePattern = new(@"(WHISPER|PRIVMSG)\s#?\w+\s:.+$", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _messageReplacePattern = new(@"^(WHISPER|PRIVMSG)\s#?\w+\s:", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    protected ChatMessage(TwitchLibMessage message)
    {
        DisplayName = message.DisplayName;
        Message = GetMessage(message).Remove(AppSettings.ChatterinoChar).TrimAll();
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
