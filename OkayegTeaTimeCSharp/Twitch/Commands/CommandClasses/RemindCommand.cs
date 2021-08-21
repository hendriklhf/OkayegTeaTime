using HLE.Collections;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages;
using OkayegTeaTimeCSharp.Utils;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses
{
    public class RemindCommand : Command
    {
        private const int _startIndex = 3;
        private const int _noMessageIndex = -1;

        public RemindCommand(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
            : base(twitchBot, chatMessage, alias)
        {
        }

        public override void Handle()
        {
            if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixHelper.GetPrefix(ChatMessage.Channel), Pattern.ReminderInTime)))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetTimedReminder(ChatMessage, GetTimedRemindMessage(), GetToTime()));
            }
            else if (ChatMessage.GetMessage().IsMatch(PatternCreator.Create(Alias, PrefixHelper.GetPrefix(ChatMessage.Channel), @"\s\w+(\s\S+)*")))
            {
                TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetReminder(ChatMessage, GetRemindMessage()));
            }
        }

        private int GetMessageStartIdx()
        {
            for (int i = _startIndex; i <= ChatMessage.GetLowerSplit().Length - 1; i++)
            {
                if (!ChatMessage.GetLowerSplit()[i].IsMatch(Pattern.TimeSplit))
                {
                    return i;
                }
            }
            return _noMessageIndex;
        }

        private long GetToTime()
        {
            int messageStartIdx = GetMessageStartIdx();
            if (messageStartIdx == _noMessageIndex)
            {
                return TimeHelper.ConvertTimeToMilliseconds(ChatMessage.GetLowerSplit()[_startIndex..].ToList());
            }
            else
            {
                return TimeHelper.ConvertTimeToMilliseconds(ChatMessage.GetLowerSplit()[_startIndex..GetMessageStartIdx()].ToList());
            }
        }

        private byte[] GetTimedRemindMessage()
        {
            int messageStartIdx = GetMessageStartIdx();
            if (messageStartIdx == _noMessageIndex)
            {
                return "".MakeInsertable();
            }
            else
            {
                string message = string.Empty;
                ChatMessage.GetSplit()[(_startIndex + ChatMessage.GetLowerSplit()[_startIndex..GetMessageStartIdx()].ToList().Count)..]
                    .ForEach(str => message += $"{str} ");
                return message.MakeInsertable();
            }
        }

        private byte[] GetRemindMessage()
        {
            string message = string.Empty;
            ChatMessage.GetSplit()[2..].ForEach(str => message += $"{str} ");
            return message.MakeInsertable();
        }
    }
}