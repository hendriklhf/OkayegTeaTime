using System.Text.RegularExpressions;
using HLE.Strings;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;
using TwitchLib = TwitchLib.Client.Models;

namespace OkayegTeaTime.Twitch.Models;

public class TwitchChatMessage : TwitchMessage
{
    public int Bits { get; }

    public double BitsInDollars { get; }

    public Channel Channel { get; }

    public ChatReply ChatReply { get; }

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

    public Noisy Noisy { get; }

    public int ChannelId { get; }

    public int SubcsribedMonthCount { get; }

    public long TmiSentTs { get; }

    public bool IsAfkCommmand => AppSettings.CommandList.AfkCommandAliases.Any(alias => CommandPattern(alias).IsMatch(Message));

    public bool IsAnyCommand => AppSettings.CommandList.AllAliases.Any(alias => CommandPattern(alias).IsMatch(Message));

    public bool IsCommand => AppSettings.CommandList.CommandAliases.Any(alias => CommandPattern(alias).IsMatch(Message));

    public bool IsIgnoredUser => AppSettings.UserLists.IgnoredUsers.Contains(UserId);

    public bool IsBotModerator => AppSettings.UserLists.Moderators.Contains(UserId);

    public string QueryableMessage => Message.RemoveSQLChars();

    private Regex CommandPattern(string alias)
    {
        return PatternCreator.Create(alias, Channel.Prefix, @"(\s|$)");
    }

    public TwitchChatMessage(TwitchLib::ChatMessage chatMessage)
        : base(chatMessage)
    {
        Bits = chatMessage.Bits;
        BitsInDollars = chatMessage.BitsInDollars;
        Channel = new(chatMessage.Channel);
        ChatReply = chatMessage.ChatReply;
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
        Noisy = chatMessage.Noisy;
        ChannelId = chatMessage.RoomId.ToInt();
        SubcsribedMonthCount = chatMessage.SubscribedMonthCount;
        TmiSentTs = chatMessage.TmiSentTs.ToLong();
    }
}
