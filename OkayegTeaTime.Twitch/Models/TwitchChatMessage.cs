using System;
using System.Linq;
using HLE;
using OkayegTeaTime.Files;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;
using TwitchLib = TwitchLib.Client.Models;

namespace OkayegTeaTime.Twitch.Models;

public sealed class TwitchChatMessage : TwitchMessage
{
    public int Bits { get; }

    public double BitsInDollars { get; }

    public ChatReply ChatReply { get; }

    public string Channel { get; }

    public CheerBadge CheerBadge { get; }

    public string CustomRewardId { get; }

    public Guid Id { get; }

    public bool IsBroadcaster { get; }

    public bool IsHighlighted { get; }

    public bool IsMe { get; }

    public bool IsModerator { get; }

    public bool IsPartner { get; }

    public bool IsSkippingSubMode { get; }

    public bool IsStaff { get; }

    public bool IsSubscriber { get; }

    public bool IsVip { get; }

    public new string Message { get; }

    public Noisy Noisy { get; }

    public long ChannelId { get; }

    public int SubcsribedMonthCount { get; }

    public long TmiSentTs { get; }

    public bool IsIgnoredUser => AppSettings.UserLists.IgnoredUsers.Contains(UserId);

    public bool IsBotModerator => AppSettings.UserLists.Moderators.Contains(UserId);

    public TwitchChatMessage(global::TwitchLib.Client.Models.ChatMessage chatMessage) : base(chatMessage)
    {
        Bits = chatMessage.Bits;
        BitsInDollars = chatMessage.BitsInDollars;
        ChatReply = chatMessage.ChatReply;
        Channel = chatMessage.Channel;
        CheerBadge = chatMessage.CheerBadge;
        CustomRewardId = chatMessage.CustomRewardId;
        Id = new(chatMessage.Id);
        IsBroadcaster = chatMessage.IsBroadcaster;
        IsHighlighted = chatMessage.IsHighlighted;
        IsMe = chatMessage.IsMe;
        IsModerator = chatMessage.IsModerator;
        IsPartner = chatMessage.IsPartner;
        IsSkippingSubMode = chatMessage.IsSkippingSubMode;
        IsStaff = chatMessage.IsStaff;
        IsSubscriber = chatMessage.IsSubscriber;
        IsVip = chatMessage.IsVip;
        Message = chatMessage.Message.Remove(AppSettings.ChatterinoChar).TrimAll();
        Noisy = chatMessage.Noisy;
        ChannelId = long.Parse(chatMessage.RoomId);
        SubcsribedMonthCount = chatMessage.SubscribedMonthCount;
        TmiSentTs = long.Parse(chatMessage.TmiSentTs);
    }
}
