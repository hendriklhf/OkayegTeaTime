using HLE.Strings;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.Data;
using OkayegTeaTimeCSharp.Messages.Enums;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;
using TwitchLib = TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Messages.Models
{
    public class TwitchChatMessage : ITwitchChatMessage
    {
        public List<string> Badges { get; }

        public int Bits { get; }

        public double BitsInDollars { get; }

        public string Channel { get; }

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
            LowerSplit = chatMessage.GetLowerSplit();
            Message = chatMessage.GetMessage();
            Noisy = chatMessage.Noisy;
            RawIrcMessage = chatMessage.RawIrcMessage;
            RoomId = chatMessage.RoomId.ToInt();
            Split = chatMessage.GetSplit();
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
            UserLists userLists = new JsonController().BotData.UserLists;
            if (userLists.Moderators.Contains(Username))
            {
                result.Add(UserTag.Moderator);
            }
            if (userLists.Owners.Contains(Username))
            {
                result.Add(UserTag.Owner);
            }
            if (userLists.SpecialUsers.Contains(Username))
            {
                result.Add(UserTag.Special);
            }
            if (userLists.SecretUsers.Contains(Username))
            {
                result.Add(UserTag.Secret);
            }
            return result;
        }
    }
}
