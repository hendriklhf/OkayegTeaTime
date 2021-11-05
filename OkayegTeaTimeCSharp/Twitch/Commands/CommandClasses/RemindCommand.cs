using HLE.Collections;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

public class RemindCommand : Command
{
    private const int _startIndex = 3;
    private const int _noMessageIndex = -1;

    public RemindCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, Pattern.ReminderInTime)))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetTimedReminder(ChatMessage, GetTimedRemindMessage(), GetToTime()));
        }
        else if (ChatMessage.Message.IsMatch(PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+(\s\S+)*")))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetReminder(ChatMessage, GetRemindMessage()));
        }
    }

    private int GetMessageStartIdx()
    {
        for (int i = _startIndex; i <= ChatMessage.LowerSplit.Length - 1; i++)
        {
            if (!ChatMessage.LowerSplit[i].IsMatch(Pattern.TimeSplit))
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
            return TimeHelper.ConvertTimeToMilliseconds(ChatMessage.LowerSplit[_startIndex..].ToList());
        }
        else
        {
            return TimeHelper.ConvertTimeToMilliseconds(ChatMessage.LowerSplit[_startIndex..GetMessageStartIdx()].ToList());
        }
    }

    private byte[] GetTimedRemindMessage()
    {
        int messageStartIdx = GetMessageStartIdx();
        if (messageStartIdx == _noMessageIndex)
        {
            return string.Empty.MakeInsertable();
        }
        else
        {
            string[] messageSplit = ChatMessage.Split[(_startIndex + ChatMessage.LowerSplit[_startIndex..GetMessageStartIdx()].ToList().Count)..];
            return messageSplit.ToSequence().MakeInsertable();
        }
    }

    private byte[] GetRemindMessage()
    {
        string[] messageSplit = ChatMessage.Split[2..];
        return messageSplit.ToSequence().MakeInsertable();
    }
}
