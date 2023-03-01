using System;
using System.Buffers;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;

namespace OkayegTeaTime.Twitch.Models;

public readonly struct ChatMessageExtension : IDisposable
{
    public SmartSplit Split { get; }

    public SmartSplit LowerSplit { get; }

    public bool IsBotModerator => CheckIfIsBotModerator();

    public bool IsIgnoredUser => CheckIfIsIgnoredUser();

    public bool IsBroadcaster => _chatMessage.UserId == _chatMessage.ChannelId;

    private readonly ChatMessage _chatMessage;
    private readonly char[] _lowerCaseMessage;

    public ChatMessageExtension(ChatMessage chatMessage)
    {
        _chatMessage = chatMessage;
        Split = new(_chatMessage.Message.AsMemory());
        _lowerCaseMessage = ArrayPool<char>.Shared.Rent(_chatMessage.Message.Length);
        _chatMessage.Message.AsSpan().ToLowerInvariant(_lowerCaseMessage);
        LowerSplit = new(_lowerCaseMessage.AsMemory()[.._chatMessage.Message.Length]);
    }

    private bool CheckIfIsBotModerator()
    {
        return AppSettings.UserLists.Moderators.Contains(_chatMessage.UserId);
    }

    private bool CheckIfIsIgnoredUser()
    {
        return AppSettings.UserLists.IgnoredUsers.Contains(_chatMessage.UserId);
    }

    public void Dispose()
    {
        ArrayPool<char>.Shared.Return(_lowerCaseMessage);
        Split.Dispose();
        LowerSplit.Dispose();
    }
}
