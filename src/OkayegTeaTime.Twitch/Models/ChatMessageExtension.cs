using System;
using System.Threading;
using HLE.Memory;
using HLE.Twitch.Tmi.Models;
using OkayegTeaTime.Configuration;

namespace OkayegTeaTime.Twitch.Models;

public struct ChatMessageExtension(ChatMessage chatMessage) : IDisposable, IEquatable<ChatMessageExtension>
{
    public SmartSplit Split
    {
        get
        {
            if (_split == SmartSplit.Empty)
            {
                _split = new(_chatMessage.Message.AsMemory());
            }

            return _split;
        }
    }

    public SmartSplit LowerSplit
    {
        get
        {
            if (_lowerSplit != SmartSplit.Empty)
            {
                return _lowerSplit;
            }

            if (_lowerCaseMessage is null)
            {
                _lowerCaseMessage = ArrayPool<char>.Shared.Rent(_chatMessage.Message.Length);
                _chatMessage.Message.AsSpan().ToLowerInvariant(_lowerCaseMessage.AsSpan());
            }

            _lowerSplit = new(_lowerCaseMessage.AsMemory(.._chatMessage.Message.Length));

            return _lowerSplit;
        }
    }

    public readonly bool IsBotModerator => GlobalSettings.Settings.Users.Moderators.Contains(_chatMessage.UserId);

    public readonly bool IsIgnoredUser => GlobalSettings.Settings.Users.IgnoredUsers.Contains(_chatMessage.UserId);

    public readonly bool IsBroadcaster => _chatMessage.UserId == _chatMessage.ChannelId;

    private readonly ChatMessage _chatMessage = chatMessage;
    private char[]? _lowerCaseMessage;
    private SmartSplit _split = SmartSplit.Empty;
    private SmartSplit _lowerSplit = SmartSplit.Empty;

    public void Dispose()
    {
        char[]? lowerCaseMessage = Interlocked.Exchange(ref _lowerCaseMessage, null);
        if (lowerCaseMessage is not null)
        {
            ArrayPool<char>.Shared.Return(lowerCaseMessage);
        }

        _split.Dispose();
        _lowerSplit.Dispose();
    }

    public readonly bool Equals(ChatMessageExtension other) => _chatMessage.Equals(other._chatMessage);

    public override readonly bool Equals(object? obj) => obj is ChatMessageExtension other && Equals(other);

    public override readonly int GetHashCode() => _chatMessage.GetHashCode();

    public static bool operator ==(ChatMessageExtension left, ChatMessageExtension right) => left.Equals(right);

    public static bool operator !=(ChatMessageExtension left, ChatMessageExtension right) => !left.Equals(right);
}
