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

    public bool IsIgnoredUser => CheckIfIsIgnoredUser();

    public bool IsBotModerator => CheckIfIsBotModerator();

    public TwitchChatMessage(global::TwitchLib.Client.Models.ChatMessage chatMessage) : base(chatMessage)
    {
        Channel = chatMessage.Channel;
        IsBroadcaster = chatMessage.IsBroadcaster;
        IsModerator = chatMessage.IsModerator;
        IsStaff = chatMessage.IsStaff;
        Message = chatMessage.Message.Remove(AppSettings.ChatterinoChar).TrimAll();
        ChannelId = long.Parse(chatMessage.RoomId);
    }

    private bool CheckIfIsIgnoredUser()
    {
        ref long firstIgnoredUser = ref MemoryMarshal.GetArrayDataReference(AppSettings.UserLists.IgnoredUsers);
        for (int i = 0; i < AppSettings.UserLists.IgnoredUsers.Length; i++)
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
        ref long firstBotModerator = ref MemoryMarshal.GetArrayDataReference(AppSettings.UserLists.Moderators);
        for (int i = 0; i < AppSettings.UserLists.IgnoredUsers.Length; i++)
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
