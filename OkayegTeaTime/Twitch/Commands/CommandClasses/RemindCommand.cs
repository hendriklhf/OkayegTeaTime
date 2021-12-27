using HLE.Collections;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

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
        var timedPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, Pattern.ReminderInTime);
        if (timedPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetTimedReminder(ChatMessage, GetTimedRemindMessage(), GetToTime()));
            return;
        }

        var nextMessagePattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+(\s\S+)*");
        if (nextMessagePattern.IsMatch(ChatMessage.Message))
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
            return string.Empty.Encode();
        }
        else
        {
            string[] messageSplit = ChatMessage.Split[(_startIndex + ChatMessage.LowerSplit[_startIndex..GetMessageStartIdx()].ToList().Count)..];
            return messageSplit.ToSequence().Encode();
        }
    }

    private byte[] GetRemindMessage()
    {
        string[] messageSplit = ChatMessage.Split[2..];
        return messageSplit.ToSequence().Encode();
    }
}
