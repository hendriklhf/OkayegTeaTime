using System;
using HLE.Memory;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;

namespace OkayegTeaTime.Twitch.Models;

public struct ChatMessageExtension(IChatMessage chatMessage) : IDisposable, IEquatable<ChatMessageExtension>
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

            if (_lowerCaseMessage == RentedArray<char>.Empty)
            {
                _lowerCaseMessage = new(_chatMessage.Message.Length);
                _chatMessage.Message.AsSpan().ToLowerInvariant(_lowerCaseMessage);
            }

            _lowerSplit = new(_lowerCaseMessage.Memory[.._chatMessage.Message.Length]);

            return _lowerSplit;
        }
    }

    public readonly bool IsBotModerator => GlobalSettings.Settings.Users.Moderators.Contains(_chatMessage.UserId);

    public readonly bool IsIgnoredUser => GlobalSettings.Settings.Users.IgnoredUsers.Contains(_chatMessage.UserId);

    public readonly bool IsBroadcaster => _chatMessage.UserId == _chatMessage.ChannelId;

    private readonly IChatMessage _chatMessage = chatMessage;
    private RentedArray<char> _lowerCaseMessage = RentedArray<char>.Empty;
    private SmartSplit _split = SmartSplit.Empty;
    private SmartSplit _lowerSplit = SmartSplit.Empty;

    public void Dispose()
    {
        _lowerCaseMessage.Dispose();
        Split.Dispose();
        LowerSplit.Dispose();
    }

    public readonly bool Equals(ChatMessageExtension other) => _lowerCaseMessage.Equals(other._lowerCaseMessage) && _split.Equals(other._split) && _lowerSplit.Equals(other._lowerSplit);

    // ReSharper disable once ArrangeModifiersOrder
    public override readonly bool Equals(object? obj) => obj is ChatMessageExtension other && Equals(other);

    // ReSharper disable once ArrangeModifiersOrder
    public override readonly int GetHashCode() => HashCode.Combine(_lowerCaseMessage, _split, _lowerSplit);

    public static bool operator ==(ChatMessageExtension left, ChatMessageExtension right) => left.Equals(right);

    public static bool operator !=(ChatMessageExtension left, ChatMessageExtension right) => !left.Equals(right);
}
