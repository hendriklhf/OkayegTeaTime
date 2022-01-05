using OkayegTeaTime.Twitch.Models;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;

namespace OkayegTeaTime.Twitch.Messages.Interfaces;

public interface ITwitchChatMessage : ITwitchMessage
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

    public bool IsAfkCommmand { get; }

    public bool IsAnyCommand { get; }

    public bool IsCommand { get; }

    public bool IsNotLoggedChannel { get; }

    public bool IsIgnoredUser { get; }

    public string QueryableMessage { get; }
}
