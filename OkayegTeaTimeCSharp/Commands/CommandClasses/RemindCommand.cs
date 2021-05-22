using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Time;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class RemindCommand
    {
        private static ChatMessage _chatMessage;

        private static readonly int _startIndex = 3;

        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            _chatMessage = chatMessage;
            if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, Pattern.ReminderInTimePattern)))
            {
                twitchBot.SendSetTimedReminder(chatMessage, GetTimedRemindMessage(), GetToTime());
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\s\w+\s\S+")))
            {
                twitchBot.SendSetReminder(chatMessage, GetRemindMessage());
            }
        }

        private static int GetMessageStartIdx()
        {
            for (int i = _startIndex; i <= _chatMessage.GetLowerSplit().Length - 1; i++)
            {
                if (!_chatMessage.GetLowerSplit()[i].IsMatch(Pattern.TimeSplitPattern))
                {
                    return i;
                }
            }
            return _startIndex + 1;
        }

        private static long GetToTime()
        {
            return TimeHelper.ConvertStringToMilliseconds(_chatMessage.GetLowerSplit()[_startIndex..GetMessageStartIdx()].ToList());
        }

        private static byte[] GetTimedRemindMessage()
        {
            string message = "";
            _chatMessage.GetSplit()[(_startIndex + _chatMessage.GetLowerSplit()[_startIndex..GetMessageStartIdx()].ToList().Count)..].ToList().ForEach(str =>
            {
                message += $"{str} ";
            });
            return message.MakeInsertable();
        }

        private static byte[] GetRemindMessage()
        {
            string message = "";
            _chatMessage.GetSplit()[2..].ToList().ForEach(str =>
            {
                message += $"{str} ";
            });
            return message.MakeInsertable();
        }
    }
}