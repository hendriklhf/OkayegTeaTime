using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HLE;
using HLE.Collections;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

public class RemindCommand : Command
{
    private static readonly Regex _targetPattern = new($@"^\S+\s{Pattern.MultipleReminderTargets}", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _timeSplitPattern = new($@"\b{Pattern.TimeSplit}\b", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    private static readonly Regex _beginningNumberPattern = new(@"^\d+", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _yearPattern = new(@"^\d+y(ear)?s?$", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _dayPattern = new(@"^\d+d(ay)?s?$", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _hourPattern = new(@"^\d+h(our)?s?$", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _minutePattern = new(@"^\d+m(in(ute)?)?s?$", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _secondPattern = new(@"^\d+s(ec(ond)?)?s?$", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

    private static readonly Regex _exceptMessagePattern =
        new($@"{_targetPattern}(\sin(\s({Pattern.TimeSplit}))+)?\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public RemindCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        string[] targets;
        string message;
        long toTime = 0;

        Regex timedPattern = PatternCreator.Create(Alias, Prefix, Pattern.ReminderInTime);
        Regex nextMessagePattern = PatternCreator.Create(Alias, Prefix, @"\s\w+(\s\S+)*.*");
        if (timedPattern.IsMatch(ChatMessage.Message))
        {
            targets = GetTargets();
            message = GetMessage();
            toTime = GetToTime();
        }
        else if (nextMessagePattern.IsMatch(ChatMessage.Message))
        {
            targets = GetTargets();
            message = GetMessage();
        }
        else
        {
            return;
        }

        Response = $"{ChatMessage.Username}, ";
        if (targets.Length == 1)
        {
            bool targetExists = TwitchApi.DoesUserExist(targets[0]);
            if (!targetExists)
            {
                Response += "the target user does not exist";
                return;
            }

            int? id = DbControl.Reminders.Add(ChatMessage.Username, targets[0], message, ChatMessage.Channel, toTime);
            if (!id.HasValue)
            {
                Response += PredefinedMessages.TooManyRemindersMessage;
                return;
            }

            Response += $"set a {(toTime == 0 ? string.Empty : "timed ")}reminder for {(targets[0] == ChatMessage.Username ? "yourself" : targets[0])} (ID: {id.Value})";
        }
        else
        {
            Dictionary<string, bool> exist = TwitchApi.DoUsersExist(targets);

            (string, string, string, string, long)[] values = targets
                .Where(t => exist[t])
                .Select<string, (string, string, string, string, long)>(t => new(ChatMessage.Username, t, message, ChatMessage.Channel, toTime))
                .ToArray();
            int?[] ids = DbControl.Reminders.AddRange(values);
            bool multi = ids.Count(i => i.HasValue) > 1;
            if (!multi)
            {
                Response += PredefinedMessages.TooManyRemindersMessage;
                return;
            }

            Response += $"set{(multi ? string.Empty : " a")} {(toTime == 0 ? string.Empty : "timed ")}reminder{(multi ? 's' : string.Empty)} for ";
            List<string> responses = new();
            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] != default && exist[targets[i]])
                {
                    responses.Add($"{targets[i]} ({ids[i]})");
                }
            }

            Response += string.Join(", ", responses);
        }
    }

    private string GetMessage()
    {
        return _exceptMessagePattern.Replace(ChatMessage.Message, string.Empty);
    }


    private long GetToTime()
    {
        MatchCollection matches = _timeSplitPattern.Matches(ChatMessage.Message);
        string[] captures = matches.Select(m => m.Value).ToArray();
        return ConvertTimeToMilliseconds(captures) + TimeHelper.Now();
    }

    private string[] GetTargets()
    {
        Match match = _targetPattern.Match(ChatMessage.Message);
        int firstWordLength = match.Value.Split()[0].Length + 1;
        string[] targets = match.Value[firstWordLength..].Remove(" ").Split(',');
        return targets.Replace(t => t.ToLower() == "me", ChatMessage.Username)
            .Select(t => t.ToLower())
            .Distinct()
            .Take(5)
            .ToArray();
    }

    private static long ConvertTimeToMilliseconds(IEnumerable<string> input)
    {
        long result = 0;
        foreach (string s in input)
        {
            if (_yearPattern.IsMatch(s))
            {
                int match = _beginningNumberPattern.Match(s).Value.ToInt();
                result += (long)TimeSpan.FromDays(365 * match).TotalMilliseconds;
            }
            else if (_dayPattern.IsMatch(s))
            {
                int match = _beginningNumberPattern.Match(s).Value.ToInt();
                result += (long)TimeSpan.FromDays(match).TotalMilliseconds;
            }
            else if (_hourPattern.IsMatch(s))
            {
                int match = _beginningNumberPattern.Match(s).Value.ToInt();
                result += (long)TimeSpan.FromHours(match).TotalMilliseconds;
            }
            else if (_minutePattern.IsMatch(s))
            {
                int match = _beginningNumberPattern.Match(s).Value.ToInt();
                result += (long)TimeSpan.FromMinutes(match).TotalMilliseconds;
            }
            else if (_secondPattern.IsMatch(s))
            {
                int match = _beginningNumberPattern.Match(s).Value.ToInt();
                result += (long)TimeSpan.FromSeconds(match).TotalMilliseconds;
            }
        }

        return result;
    }
}
