using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HLE;
using OkayegTeaTime.Files;

namespace OkayegTeaTime.Twitch.Models;

public sealed class TwitchChatMessage : TwitchMessage
{
    public string Channel { get; }

    public bool IsBroadcaster { get; }

    public bool IsModerator { get; }

    public bool IsStaff { get; }

    public new string Message { get; }

    public long ChannelId { get; }

    public long TmiSentTs { get; }

    public bool IsIgnoredUser => CheckIfIsIgnoredUser();

    public bool IsBotModerator => CheckIfIsBotModerator();

    public TwitchChatMessage(global::TwitchLib.Client.Models.ChatMessage chatMessage) : base(chatMessage)
    {
        Channel = chatMessage.Channel;
        IsBroadcaster = chatMessage.IsBroadcaster;
        IsModerator = chatMessage.IsModerator;
        IsStaff = chatMessage.IsStaff;
        Message = chatMessage.Message.Replace(AppSettings.ChatterinoChar, string.Empty).TrimAll();
        ChannelId = long.Parse(chatMessage.RoomId);
        TmiSentTs = long.Parse(chatMessage.TmiSentTs, NumberStyles.Integer, CultureInfo.InvariantCulture);
    }

    private bool CheckIfIsIgnoredUser()
    {
        long[] ignoredUsers = AppSettings.UserLists.IgnoredUsers;
        ref long firstIgnoredUser = ref MemoryMarshal.GetArrayDataReference(ignoredUsers);
        int userLength = AppSettings.UserLists.IgnoredUsers.Length;
        for (int i = 0; i < userLength; i++)
        {
            long ignoredUser = Unsafe.Add(ref firstIgnoredUser, i);
            if (ignoredUser == UserId)
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckIfIsBotModerator()
    {
        long[] moderators = AppSettings.UserLists.Moderators;
        ref long firstBotModerator = ref MemoryMarshal.GetArrayDataReference(AppSettings.UserLists.Moderators);
        int moderatorsLength = moderators.Length;
        for (int i = 0; i < moderatorsLength; i++)
        {
            long ignoredUser = Unsafe.Add(ref firstBotModerator, i);
            if (ignoredUser == UserId)
            {
                return true;
            }
        }

        return false;
    }
}
