using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HLE;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Remind)]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public readonly unsafe ref struct RemindCommand
{
    public TwitchChatMessage ChatMessage { get; }

    public StringBuilder* Response { get; }

    private readonly TwitchBot _twitchBot;
    private readonly string? _prefix;
    private readonly string _alias;

    private readonly long _now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    private const string _timeIdentificator = "in";
    private const string _yourself = "me";

    [TimePattern(nameof(TimeSpan.FromDays), 365)]
    private static readonly Regex _yearPattern = new(@"\b\d+([,\.]\d+)?\s?y(ear)?s?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    [TimePattern(nameof(TimeSpan.FromDays), 7)]
    private static readonly Regex _weekPattern = new(@"\b\d+([,\.]\d+)?\s?w(eek)?s?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    [TimePattern(nameof(TimeSpan.FromDays))]
    private static readonly Regex _dayPattern = new(@"\b\d+([,\.]\d+)?\s?d(ay)?s?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    [TimePattern(nameof(TimeSpan.FromHours))]
    private static readonly Regex _hourPattern = new(@"\b\d+([,\.]\d+)?\s?h(our)?s?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    [TimePattern(nameof(TimeSpan.FromMinutes))]
    private static readonly Regex _minutePattern = new(@"\b\d+([,\.]\d+)?\s?m(in(ute)?)?s?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    [TimePattern(nameof(TimeSpan.FromSeconds))]
    private static readonly Regex _secondPattern = new(@"\b\d+([,\.]\d+)?\s?s(ec(ond)?)?s?\b", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    private static readonly Regex _beginningNumberPattern = new(@"^\d+([,\.]\d+)?", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly string _timePattern = GetTimePattern();
    private static readonly TimeConversionMethod[] _timeConversions = GetTimeConversionMethods();
    private static readonly Regex _targetPattern = new($@"^\S+\s{Pattern.MultipleTargets}", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly short _minimumTimedReminderTime = (short)TimeSpan.FromSeconds(30).TotalMilliseconds;

    private static readonly Regex _exceptMessagePattern = new($@"^\S+\s((\w{{3,25}})|(me))(,\s?((\w{{3,25}})|(me)))*(\sin\s({_timePattern})(\s{_timePattern})*)?\s?", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    public RemindCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, StringBuilder* response, string? prefix, string alias)
    {
        ChatMessage = chatMessage;
        Response = response;
        _twitchBot = twitchBot;
        _prefix = prefix;
        _alias = alias;
    }

    public void Handle()
    {
        string[] targets;
        string message;
        long toTime = 0;

        Regex timedReminderPattern = _twitchBot.RegexCreator.Create(_alias, _prefix, $@"\s((\w{{3,25}})|(me))(,\s?((\w{{3,25}})|(me)))*\sin\s({_timePattern})(\s{_timePattern})*.*");
        Regex normalReminderPattern = _twitchBot.RegexCreator.Create(_alias, _prefix, @"\s((\w{3,25})|(me))(,\s?((\w{3,25})|(me)))*.*");
        if (timedReminderPattern.IsMatch(ChatMessage.Message))
        {
            toTime = GetToTime();
            if (toTime < _minimumTimedReminderTime + _now)
            {
                Response->Append(ChatMessage.Username, Messages.CommaSpace, Messages.TheMinimumTimeForATimedReminderIs30S);
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

        Response->Append(ChatMessage.Username, Messages.CommaSpace);
        if (targets.Length == 1)
        {
            bool targetExists = _twitchBot.TwitchApi.DoesUserExist(targets[0]);
            if (!targetExists)
            {
                Response->Append(Messages.TheTargetUserDoesNotExist);
                return;
            }

            Reminder reminder = new(ChatMessage.Username, targets[0], message, ChatMessage.Channel, toTime);
            int id = _twitchBot.Reminders.Add(reminder);
            if (id == -1)
            {
                Response->Append(Messages.ThatUserHasTooManyRemindersSetForThem);
                return;
            }

            Response->Append("set a ", toTime == 0 ? string.Empty : "timed ", "reminder for ", targets[0] == ChatMessage.Username ? "yourself" : targets[0]);
            Response->Append(" (ID: ");
            Response->Append(id);
            Response->Append(')');
        }
        else
        {
            Dictionary<string, bool> exist = _twitchBot.TwitchApi.DoUsersExist(targets);
            TwitchChatMessage chatMessage = ChatMessage;
            Reminder[] reminders = targets.Where(t => exist[t]).Select(t => new Reminder(chatMessage.Username, t, message, chatMessage.Channel, toTime)).ToArray();
            if (reminders.Length == 0)
            {
                Response->Append(Messages.AllTargetUsersDoNotExist);
                return;
            }

            int[] ids = _twitchBot.Reminders.AddRange(reminders);
            int count = ids.Count(i => i != -1);
            if (count == 0)
            {
                Response->Append(Messages.AllTargetUsersHaveTooManyRemindersSetForThem);
                return;
            }

            bool multi = count > 1;
            Response->Append("set", multi ? string.Empty : " a", StringHelper.Whitespace, toTime == 0 ? string.Empty : "timed ");
            Response->Append("reminder", multi ? "s" : string.Empty, " for ");
            string[] responses = ids.Select(i =>
            {
                Reminder? reminder = reminders.FirstOrDefault(r => r.Id == i);
                return reminder is null ? null : $"{reminder.Target} ({reminder.Id})";
            }).Where(r => r is not null).ToArray()!;

            Span<char> joinBuffer = stackalloc char[500];
            int bufferLength = StringHelper.Join(responses, Messages.CommaSpace, joinBuffer);
            Response->Append(joinBuffer[..bufferLength]);
        }
    }

    private string GetMessage()
    {
        return _exceptMessagePattern.Replace(ChatMessage.Message, string.Empty);
    }

    private long GetToTime()
    {
        long result = 0;
        int timeStartIndex = GetTimeStartIdx();
        string times = string.Join(' ', ChatMessage.Split, timeStartIndex, ChatMessage.Split.Length - timeStartIndex);
        string[] matches = _timeConversions.Select(t => t.Regex).Select(r => r.Matches(times).Select(m => m.Value)).SelectMany(m => m).ToArray();
        foreach (string match in matches)
        {
            foreach (TimeConversionMethod timeConversion in _timeConversions)
            {
                if (!timeConversion.Regex.IsMatch(match))
                {
                    continue;
                }

                string number = _beginningNumberPattern.Match(match).Value;
                number = number.Replace(',', '.');
                double d = double.Parse(number) * timeConversion.Factor;
                TimeSpan span = timeConversion.Method(d);
                result += (long)Math.Round(span.TotalMilliseconds);
            }
        }

        return result + _now;
    }

    private string[] GetTargets()
    {
        ReadOnlySpan<char> match = _targetPattern.Match(ChatMessage.Message).Value;
        int firstWordLength = match[match.GetRangesOfSplit()[0]].Length + 1;
        match = match[firstWordLength..];
        ReadOnlySpan<Range> ranges = match.GetRangesOfSplit(',');
        string[] targets = new string[ranges.Length];
        Span<char> targetBuffer = stackalloc char[30];
        for (int i = 0; i < ranges.Length; i++)
        {
            ReadOnlySpan<char> targetSpan = match[ranges[i]].Trim();
            int targetLength = targetSpan.ToLowerInvariant(targetBuffer);
            string target = new(targetBuffer[..targetLength]);
            targets[i] = target == _yourself ? ChatMessage.Username : target;
        }

        return targets.Distinct().Take(5).ToArray();
    }

    private static string GetTimePattern()
    {
        IEnumerable<string> pattern = typeof(RemindCommand).GetFields(BindingFlags.Static | BindingFlags.NonPublic).Where(f => f.GetCustomAttribute<TimePatternAttribute>() is not null).Select(f => f.GetValue(null)!.ToString()![2..^2]);
        return '(' + string.Join(")|(", pattern) + ')';
    }

    private static TimeConversionMethod[] GetTimeConversionMethods()
    {
        FieldInfo[] fields = typeof(RemindCommand).GetFields(BindingFlags.Static | BindingFlags.NonPublic).Where(f => f.GetCustomAttribute<TimePatternAttribute>() is not null).ToArray();
        string[] methodNames = fields.Select(f => f.GetCustomAttribute<TimePatternAttribute>()!.ConversionMethod).ToArray();
        MethodInfo[] methods = typeof(TimeSpan).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(f => methodNames.Contains(f.Name)).ToArray();
        return fields.Select(f =>
        {
            TimePatternAttribute attr = f.GetCustomAttribute<TimePatternAttribute>()!;
            Regex regex = (Regex)f.GetValue(null)!;
            MethodInfo method = methods.First(m => m.Name == attr.ConversionMethod);
            return new TimeConversionMethod(regex, (delegate*<double, TimeSpan>)method.MethodHandle.GetFunctionPointer(), attr.Factor);
        }).ToArray();
    }

    private int GetTimeStartIdx()
    {
        for (int i = 2; i < ChatMessage.LowerSplit.Length; i++)
        {
            if (ChatMessage.LowerSplit[i] == _timeIdentificator)
            {
                return i + 1;
            }
        }

        return -1;
    }
}
