using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HLE.Collections;
using HLE.Time;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Remind)]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public sealed class RemindCommand : Command
{
    private readonly long _now = TimeHelper.Now();

    private const string _timeIndentificator = "in";
    private const string _yourself = "me";

    [TimePattern(nameof(TimeSpan.FromDays), 365)]
    private static readonly Regex _yearPattern = new(@"\b\d+([,\.]\d+)?\s?y(ear)?s?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

    [TimePattern(nameof(TimeSpan.FromDays), 7)]
    private static readonly Regex _weekPattern = new(@"\b\d+([,\.]\d+)?\s?w(eek)?s?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

    [TimePattern(nameof(TimeSpan.FromDays))]
    private static readonly Regex _dayPattern = new(@"\b\d+([,\.]\d+)?\s?d(ay)?s?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

    [TimePattern(nameof(TimeSpan.FromHours))]
    private static readonly Regex _hourPattern = new(@"\b\d+([,\.]\d+)?\s?h(our)?s?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

    [TimePattern(nameof(TimeSpan.FromMinutes))]
    private static readonly Regex _minutePattern = new(@"\b\d+([,\.]\d+)?\s?m(in(ute)?)?s?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

    [TimePattern(nameof(TimeSpan.FromSeconds))]
    private static readonly Regex _secondPattern = new(@"\b\d+([,\.]\d+)?\s?s(ec(ond)?)?s?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

    private static readonly Regex _beginningNumberPattern = new(@"^\d+([,\.]\d+)?", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static readonly string _timePattern = GetTimePattern();
    private static readonly TimeConversionMethod[] _timeConversions = GetTimeConversionMethods();
    private static readonly Regex _targetPattern = new($@"^\S+\s{Pattern.MultipleTargets}", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static readonly short _minimumTimedReminderTime = (short)TimeSpan.FromSeconds(30).TotalMilliseconds;

    private static readonly Regex _exceptMessagePattern = new($@"^\S+\s((\w{{3,25}})|(me))(,\s?((\w{{3,25}})|(me)))*(\sin\s({_timePattern})(\s{_timePattern})*)?\s?", RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(500));

    public RemindCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        string[] targets;
        string message;
        long toTime = 0;

        Regex timedReminderPattern = PatternCreator.Create(_alias, _prefix, $@"\s((\w{{3,25}})|(me))(,\s?((\w{{3,25}})|(me)))*\sin\s({_timePattern})(\s{_timePattern})*.*");
        Regex normalReminderPattern = PatternCreator.Create(_alias, _prefix, @"\s((\w{3,25})|(me))(,\s?((\w{3,25})|(me)))*.*");
        if (timedReminderPattern.IsMatch(ChatMessage.Message))
        {
            toTime = GetToTime();
            if (toTime < _minimumTimedReminderTime + _now)
            {
                Response = $"{ChatMessage.Username}, the minimum time for a timed a reminder is 30s";
                return;
            }

            targets = GetTargets();
            message = GetMessage();
        }
        else if (normalReminderPattern.IsMatch(ChatMessage.Message))
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
            bool targetExists = _twitchBot.TwitchApi.DoesUserExist(targets[0]);
            if (!targetExists)
            {
                Response += "the target user does not exist";
                return;
            }

            Reminder reminder = new(ChatMessage.Username, targets[0], message, ChatMessage.Channel, toTime);
            int id = _twitchBot.Reminders.Add(reminder);
            if (id == -1)
            {
                Response += PredefinedMessages.TooManyRemindersMessage;
                return;
            }

            Response += $"set a {(toTime == 0 ? string.Empty : "timed ")}reminder for {(targets[0] == ChatMessage.Username ? "yourself" : targets[0])} (ID: {id})";
        }
        else
        {
            Dictionary<string, bool> exist = _twitchBot.TwitchApi.DoUsersExist(targets);
            Reminder[] reminders = targets.Where(t => exist[t]).Select(t => new Reminder(ChatMessage.Username, t, message, ChatMessage.Channel, toTime)).ToArray();
            if (reminders.Length == 0)
            {
                Response += "all target users do not exist";
                return;
            }

            int[] ids = _twitchBot.Reminders.AddRange(reminders);
            int count = ids.Count(i => i != -1);
            if (count == 0)
            {
                Response += "all target users have too many reminders set for them";
                return;
            }

            bool multi = count > 1;
            Response += $"set{(multi ? string.Empty : " a")} {(toTime == 0 ? string.Empty : "timed ")}reminder{(multi ? 's' : string.Empty)} for ";
            string[] responses = ids.Select(i =>
            {
                Reminder? reminder = reminders.FirstOrDefault(r => r.Id == i);
                return reminder is null ? null : $"{reminder.Target} ({reminder.Id})";
            }).Where(r => r is not null).ToArray()!;

            Response += string.Join(", ", responses);
        }
    }

    private string GetMessage()
    {
        return _exceptMessagePattern.Replace(ChatMessage.Message, string.Empty);
    }

    private long GetToTime()
    {
        long result = 0;
        string times = ChatMessage.Split[GetTimeStartIdx()..].JoinToString(' ');
        string[] matches = _timeConversions.Select(t => t.Regex).Select(r => r.Matches(times).Select(m => m.Value)).SelectEach().ToArray();
        foreach (string match in matches)
        {
            foreach (TimeConversionMethod timeConversion in _timeConversions)
            {
                if (!timeConversion.Regex.IsMatch(match))
                {
                    continue;
                }

                string number = _beginningNumberPattern.Match(match).Value;
                double d = double.Parse(number.Replace(',', '.')) * timeConversion.Factor;
                TimeSpan span = (TimeSpan)timeConversion.Method.Invoke(null, new object?[]
                {
                    d
                })!;
                result += (long)Math.Round(span.TotalMilliseconds);
            }
        }

        return result + _now;
    }

    private string[] GetTargets()
    {
        Match match = _targetPattern.Match(ChatMessage.Message);
        int firstWordLength = match.Value.Split()[0].Length + 1;
        string[] targets = match.Value[firstWordLength..].Split(',').Select(t => t.Trim()).ToArray();
        return targets.Replace(t => t.ToLower() == _yourself, ChatMessage.Username).Select(t => t.ToLower()).Distinct().Take(5).ToArray();
    }

    private static string GetTimePattern()
    {
        IEnumerable<string> pattern =
            typeof(RemindCommand).GetFields(BindingFlags.Static | BindingFlags.NonPublic).Where(f => f.GetCustomAttribute<TimePattern>() is not null).Select(f => f.GetValue(null)!.ToString()![2..^2]);
        return '(' + string.Join(")|(", pattern) + ')';
    }

    private static TimeConversionMethod[] GetTimeConversionMethods()
    {
        FieldInfo[] fields = typeof(RemindCommand).GetFields(BindingFlags.Static | BindingFlags.NonPublic).Where(f => f.GetCustomAttribute<TimePattern>() is not null).ToArray();
        string[] methodNames = fields.Select(f => f.GetCustomAttribute<TimePattern>()!.ConversionMethod).ToArray();
        MethodInfo[] methods = typeof(TimeSpan).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(f => methodNames.Contains(f.Name)).ToArray();
        return fields.Select(f =>
        {
            TimePattern attr = f.GetCustomAttribute<TimePattern>()!;
            Regex regex = (f.GetValue(null) as Regex)!;
            MethodInfo method = methods.First(m => m.Name == attr.ConversionMethod);
            return new TimeConversionMethod(regex, method, attr.Factor);
        }).ToArray();
    }

    private int GetTimeStartIdx()
    {
        for (int i = 2; i < ChatMessage.LowerSplit.Length; i++)
        {
            if (ChatMessage.LowerSplit[i] == _timeIndentificator)
            {
                return i + 1;
            }
        }

        return -1;
    }
}
