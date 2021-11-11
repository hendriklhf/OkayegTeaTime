using System.Drawing;
using HLE.Strings;
using OkayegTeaTimeCSharp.Twitch.Messages.Enums;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;
using TwitchLib = TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Models;

public class TwitchChatMessage : ITwitchChatMessage
{
    public List<string> Badges { get; }

    public int Bits { get; }

    public double BitsInDollars { get; }

    public Channel Channel { get; }

    public ChatReply ChatReply { get; }

    public CheerBadge CheerBadge { get; }

    public Color Color { get; }

    public string ColorHex { get; }

    public string CustomRewardId { get; }

    public string DisplayName { get; }

    public Guid Id { get; }

    public bool IsBroadcaster { get; }

    public bool IsHighlighted { get; }

    public bool IsMe { get; }

    public bool IsModerator { get; }

    public bool IsPartner { get; }

    public bool IsSkippingSubMode { get; }

    public bool IsStaff { get; }

    public bool IsSubscriber { get; }

    public bool IsTurbo { get; }

    public bool IsVip { get; }

    public string[] LowerSplit { get; }

    public string Message { get; }

    public Noisy Noisy { get; }

    public string RawIrcMessage { get; }

    public int RoomId { get; }

    public string[] Split { get; }

    public int SubcsribedMonthCount { get; }

    public long TmiSentTs { get; }

    public int UserId { get; }

    public string Username { get; }

    public List<UserTag> UserTags { get; }

    public UserType UserType { get; }

    public bool IsAfkCommmand => CommandList.AfkCommandAliases.Any(alias => Message.IsMatch(PatternCreator.Create(alias, Channel.Prefix, @"(\s|$)")));

    public bool IsAnyCommand => CommandList.AllAliases.Any(alias => Message.IsMatch(PatternCreator.Create(alias, Channel.Prefix, @"(\s|$)")));

    public bool IsCommand => CommandList.CommandAliases.Any(alias => Message.IsMatch(PatternCreator.Create(alias, Channel.Prefix, @"(\s|$)")));

    public bool IsNotLoggedChannel => Settings.NotLoggedChannels.Contains(Channel.Name);

    public bool IsIgnoredUser => Settings.UserLists.IgnoredUsers.Contains(Username);

    public string QueryableMessage => Message.RemoveSQLChars();

    public TwitchChatMessage(TwitchLib::ChatMessage chatMessage)
    {
        Badges = chatMessage.Badges.Select(b => b.Key).ToList();
        Bits = chatMessage.Bits;
        BitsInDollars = chatMessage.BitsInDollars;
        Channel = chatMessage.Channel;
        ChatReply = chatMessage.ChatReply;
        CheerBadge = chatMessage.CheerBadge;
        Color = chatMessage.Color;
        ColorHex = chatMessage.ColorHex;
        CustomRewardId = chatMessage.CustomRewardId;
        DisplayName = chatMessage.DisplayName;
        Id = new(chatMessage.Id);
        IsBroadcaster = chatMessage.IsBroadcaster;
        IsHighlighted = chatMessage.IsHighlighted;
        IsMe = chatMessage.IsMe;
        IsModerator = chatMessage.IsModerator;
        IsPartner = chatMessage.IsPartner;
        IsSkippingSubMode = chatMessage.IsSkippingSubMode;
        IsStaff = chatMessage.IsStaff;
        IsSubscriber = chatMessage.IsSubscriber;
        IsTurbo = chatMessage.IsTurbo;
        IsVip = chatMessage.IsVip;
        Message = chatMessage.Message.RemoveChatterinoChar();
        LowerSplit = Message.ToLower().Split();
        Noisy = chatMessage.Noisy;
        RawIrcMessage = chatMessage.RawIrcMessage;
        RoomId = chatMessage.RoomId.ToInt();
        Split = Message.Split();
        SubcsribedMonthCount = chatMessage.SubscribedMonthCount;
        TmiSentTs = chatMessage.TmiSentTs.ToLong();
        UserId = chatMessage.UserId.ToInt();
        Username = chatMessage.Username;
        UserTags = GetUserTags();
        UserType = chatMessage.UserType;
    }

    private List<UserTag> GetUserTags()
    {
        List<UserTag> result = new() { UserTag.Normal };
        if (Settings.UserLists.Moderators.Contains(Username))
        {
            result.Add(UserTag.Moderator);
        }
        if (Settings.UserLists.Owners.Contains(Username))
        {
            result.Add(UserTag.Owner);
        }
        if (Settings.UserLists.IgnoredUsers.Contains(Username))
        {
            result.Add(UserTag.Special);
        }
        if (Settings.UserLists.SecretUsers.Contains(Username))
        {
            result.Add(UserTag.Secret);
        }
        return result;
    }
}
