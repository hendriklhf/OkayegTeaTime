using HLE.Strings;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Twitch.Messages;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Messages.Models
{
    public class ChatMessage : IChatMessage
    {
        public string DisplayName { get; }

        public List<string> LowerSplit { get; }

        public string Message { get; }

        public List<string> Split { get; }

        public string Username { get; }

        public ChatMessage(TwitchLibMessage twitchLibMessage)
        {
            DisplayName = twitchLibMessage.DisplayName;
            Message = GetMessage(twitchLibMessage);
            LowerSplit = GetLowerSplit();
            Split = GetSplit();
            Username = twitchLibMessage.Username;
        }

        private string GetMessage(TwitchLibMessage twitchLibMessage)
        {
            string message = twitchLibMessage.RawIrcMessage.Match(@"(WHISPER|PRIVMSG)\s#?\w+\s:.+$");
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
