using HLE.Strings;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.Data;
using OkayegTeaTimeCSharp.Messages.Enums;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Messages.Models
{
    public class TwitchMessage : ITwitchMessage
    {
        public List<string> Badges { get; }

        public Color Color { get; }

        public string ColorHex { get; }

        public bool IsTurbo { get; }

        public string RawIrcMessage { get; }

        public int UserId { get; }

        public List<UserTag> UserTags { get; }

        public UserType UserType { get; }

        public string DisplayName { get; }

        public List<string> LowerSplit { get; }

        public string Message { get; }

        public List<string> Split { get; }

        public string Username { get; }

        public TwitchMessage(TwitchLibMessage twitchLibMessage)
        {
            Badges = twitchLibMessage.Badges.Select(b => b.Key).ToList();
            Color = twitchLibMessage.Color;
            ColorHex = twitchLibMessage.ColorHex;
            DisplayName = twitchLibMessage.DisplayName;
            IsTurbo = twitchLibMessage.IsTurbo;
            RawIrcMessage = twitchLibMessage.RawIrcMessage;
            Message = GetMessage();
            LowerSplit = GetLowerSplit();
            Split = GetSplit();
            UserId = twitchLibMessage.UserId.ToInt();
            Username = twitchLibMessage.Username;
            UserTags = GetUserTags();
            UserType = twitchLibMessage.UserType;
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

        private string GetMessage()
        {
            string message = RawIrcMessage.Match(@"(WHISPER|PRIVMSG)\s#?\w+\s:.+$");
            return message.ReplacePattern(@"^(WHISPER|PRIVMSG)\s#?\w+\s:", "");
        }

        private List<string> GetSplit()
        {
            return Message.SplitNormal().ToList();
        }

        private List<string> GetLowerSplit()
        {
            return Message.SplitToLowerCase().ToList();
        }
    }
}
