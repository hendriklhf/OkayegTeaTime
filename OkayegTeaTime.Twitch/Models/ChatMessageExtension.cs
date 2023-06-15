using System;
using HLE.Memory;
using HLE.Twitch.Models;
using OkayegTeaTime.Settings;

namespace OkayegTeaTime.Twitch.Models;

public struct ChatMessageExtension : IDisposable
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

    public readonly bool IsBotModerator => CheckIfIsBotModerator();

    public readonly bool IsIgnoredUser => CheckIfIsIgnoredUser();

    public readonly bool IsBroadcaster => _chatMessage.UserId == _chatMessage.ChannelId;

    private readonly ChatMessage _chatMessage;
    private RentedArray<char> _lowerCaseMessage = RentedArray<char>.Empty;
    private SmartSplit _split = SmartSplit.Empty;
    private SmartSplit _lowerSplit = SmartSplit.Empty;

    public ChatMessageExtension(ChatMessage chatMessage)
    {
        _chatMessage = chatMessage;
    }

    private readonly bool CheckIfIsBotModerator()
    {
        return AppSettings.UserLists.Moderators.Contains(_chatMessage.UserId);
    }

    private readonly bool CheckIfIsIgnoredUser()
    {
        return AppSettings.UserLists.IgnoredUsers.Contains(_chatMessage.UserId);
    }

    public void Dispose()
    {
        _lowerCaseMessage.Dispose();
        Split.Dispose();
        LowerSplit.Dispose();
    }
}
