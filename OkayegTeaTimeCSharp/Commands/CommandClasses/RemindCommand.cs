using HLE.Collections;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTimeCSharp.Messages;
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
        private const int _startIndex = 3;
        private const int _noMessageIndex = -1;

        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            _chatMessage = chatMessage;
            if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), Pattern.ReminderInTime)))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendSetTimedReminder(chatMessage, GetTimedRemindMessage(), GetToTime()));
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s\w+(\s\S+)*")))
            {
                twitchBot.Send(chatMessage.Channel, BotActions.SendSetReminder(chatMessage, GetRemindMessage()));
            }
        }

        private static int GetMessageStartIdx()
        {
            for (int i = _startIndex; i <= _chatMessage.GetLowerSplit().Length - 1; i++)
            {
                if (!_chatMessage.GetLowerSplit()[i].IsMatch(Pattern.TimeSplit))
                {
                    return i;
                }
            }
            return _noMessageIndex;
        }

        private static long GetToTime()
        {
            int messageStartIdx = GetMessageStartIdx();
            if (messageStartIdx == _noMessageIndex)
            {
                return TimeHelper.ConvertTimeToMilliseconds(_chatMessage.GetLowerSplit()[_startIndex..].ToList());
            }
            else
            {
                return TimeHelper.ConvertTimeToMilliseconds(_chatMessage.GetLowerSplit()[_startIndex..GetMessageStartIdx()].ToList());
            }
        }

        private static byte[] GetTimedRemindMessage()
        {
            int messageStartIdx = GetMessageStartIdx();
            if (messageStartIdx == _noMessageIndex)
            {
                return "".MakeInsertable();
            }
            else
            {
                string message = "";
                _chatMessage.GetSplit()[(_startIndex + _chatMessage.GetLowerSplit()[_startIndex..GetMessageStartIdx()].ToList().Count)..].ForEach(str =>
                {
                    message += $"{str} ";
                });
                return message.MakeInsertable();
            }
        }

        private static byte[] GetRemindMessage()
        {
            string message = "";
            _chatMessage.GetSplit()[2..].ForEach(str =>
            {
                message += $"{str} ";
            });
            return message.MakeInsertable();
        }
    }
}