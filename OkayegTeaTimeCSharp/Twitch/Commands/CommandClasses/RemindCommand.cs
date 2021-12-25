using System.Text.RegularExpressions;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

public class RemindCommand : Command
{
    private const byte _startIndex = 3;
    private const sbyte _noMessageIndex = -1;

    private static readonly Regex _targetPattern = new($@"^\S+\s{Pattern.MultipleReminderTargets}", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _timeSplitPattern = new(Pattern.TimeSplit, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _exceptMessagePattern = new($@"{_targetPattern}\s(in\s({Pattern.TimeSplit}\s)+)?", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public RemindCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        string[] targets = GetTargets();
        string message = GetMessage();

        Regex timedPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, Pattern.ReminderInTime);
        if (timedPattern.IsMatch(ChatMessage.Message))
        {
            long toTime = GetToTime();
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetTimedReminder(ChatMessage, targets, message, toTime));
            return;
        }

        Regex nextMessagePattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+(\s\S+)*");
        if (nextMessagePattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetReminder(ChatMessage, targets, message));
        }
    }

    private string GetMessage()
    {
        return _exceptMessagePattern.Replace(ChatMessage.Message, string.Empty);
    }


    private long GetToTime()
    {
        MatchCollection matches = _timeSplitPattern.Matches(ChatMessage.Message);
        List<string> captures = matches.Select(m => m.Value).ToList();
        return TimeHelper.ConvertTimeToMilliseconds(captures) + TimeHelper.Now();
    }

    private string[] GetTargets()
    {
        string[] targets = Array.Empty<string>();
        Match match = _targetPattern.Match(ChatMessage.Message);
        if (!string.IsNullOrEmpty(match.Value))
        {
            int firstWordLength = match.Value.Split()[0].Length + 1;
            targets = match.Value[firstWordLength..].Remove(" ").Split(',');
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].ToLower() == "me")
                {
                    targets[i] = ChatMessage.Username;
                }
            }
        }
        return targets.Take(5).ToArray();
    }
}
