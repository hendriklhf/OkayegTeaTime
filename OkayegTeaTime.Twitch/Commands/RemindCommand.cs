using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;
using StringHelper = HLE.Strings.StringHelper;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Remind, typeof(RemindCommand))]
public readonly struct RemindCommand(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias)
    : IChatCommand<RemindCommand>
{
    public PooledStringBuilder Response { get; } = new(GlobalSettings.MaxMessageLength);

    public IChatMessage ChatMessage { get; } = chatMessage;

    private readonly TwitchBot _twitchBot = twitchBot;
    private readonly ReadOnlyMemory<char> _prefix = prefix;
    private readonly ReadOnlyMemory<char> _alias = alias;

    private readonly long _now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    private const string _timeIdentificator = "in";
    private const string _yourself = "me";

#pragma warning disable CA1823 // unused field
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
#pragma warning restore CA1823

    private static readonly Regex _beginningNumberPattern = new(@"^\d+([,\.]\d+)?", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly string _timePattern = GetTimePattern();
    private static readonly TimeConversionMethod[] _timeConversions = GetTimeConversionMethods();
    private static readonly Regex _targetPattern = new($@"^\S+\s{Pattern.MultipleTargets}", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly short _minimumTimedReminderTime = (short)TimeSpan.FromSeconds(30).TotalMilliseconds;

    private static readonly Regex _exceptMessagePattern = new($@"^\S+\s((\w{{3,25}})|(me))(,\s?((\w{{3,25}})|(me)))*(\sin\s({_timePattern})(\s{_timePattern})*)?\s?", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    public static void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out RemindCommand command)
        => command = new(twitchBot, chatMessage, prefix, alias);

    public async ValueTask HandleAsync()
    {
        string[] targets;
        string message;
        long toTime = 0;

        Regex timedReminderPattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, $@"\s((\w{{3,25}})|(me))(,\s?((\w{{3,25}})|(me)))*\sin\s({_timePattern})(\s{_timePattern})*.*");
        Regex normalReminderPattern = _twitchBot.MessageRegexCreator.Create(_alias.Span, _prefix.Span, @"\s((\w{3,25})|(me))(,\s?((\w{3,25})|(me)))*.*");
        if (timedReminderPattern.IsMatch(ChatMessage.Message))
        {
            toTime = GetToTime();
            if (toTime < _minimumTimedReminderTime + _now)
            {
                Response.Append(ChatMessage.Username, ", ", Messages.TheMinimumTimeForATimedReminderIs30S);
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

        Response.Append(ChatMessage.Username, ", ");
        if (targets.Length == 1)
        {
            var targetUser = await _twitchBot.TwitchApi.GetUserAsync(targets[0]);
            if (targetUser is null)
            {
                Response.Append(Messages.TheTargetUserDoesNotExist);
                return;
            }

            Reminder reminder = new(ChatMessage.Username, targets[0], message, ChatMessage.Channel, toTime);
            int id = _twitchBot.Reminders.Add(reminder);
            if (id == -1)
            {
                Response.Append(Messages.ThatUserHasTooManyRemindersSetForThem);
                return;
            }

            Response.Append("set a ", toTime == 0 ? string.Empty : "timed ", "reminder for ", targets[0] == ChatMessage.Username ? "yourself" : targets[0]);
            Response.Append(" (ID: ");
            Response.Append(id);
            Response.Append(')');
            return;
        }

        var targetUsers = await _twitchBot.TwitchApi.GetUsersAsync(targets);
        Reminder[] reminders = new Reminder[targetUsers.Length];
        for (int i = 0; i < targetUsers.Length; i++)
        {
            var targetUser = targetUsers[i];
            reminders[i] = new(ChatMessage.Username, targetUser.Username, message, ChatMessage.Channel, toTime);
        }

        if (reminders.Length == 0)
        {
            Response.Append(Messages.AllTargetUsersDoNotExist);
            return;
        }

        int[] ids = _twitchBot.Reminders.AddRange(reminders);
        int count = ids.Count(static i => i > 0);
        if (count == 0)
        {
            Response.Append(Messages.AllTargetUsersHaveTooManyRemindersSetForThem);
            return;
        }

        bool multi = count > 1;
        Response.Append("set", multi ? string.Empty : " a", " ", toTime == 0 ? string.Empty : "timed ");
        Response.Append("reminder", multi ? "s" : string.Empty, " for ");
        string[] responses = ids.Select(i =>
        {
            Reminder? reminder = reminders.FirstOrDefault(r => r.Id == i);
            return reminder is null ? null : $"{reminder.Target} ({reminder.Id})";
        }).Where(static r => r is not null).ToArray()!;

        int joinLength = StringHelper.Join(responses, ", ", Response.FreeBufferSpan);
        Response.Advance(joinLength);
    }

    private string GetMessage() => _exceptMessagePattern.Replace(ChatMessage.Message, string.Empty);

    [SkipLocalsInit]
    private unsafe long GetToTime()
    {
        long result = 0;
        using ChatMessageExtension messageExtension = new(ChatMessage);
        int timeStartIndex = GetTimeStartIndex(&messageExtension);

        ReadOnlySpan<ReadOnlyMemory<char>> splits = messageExtension.LowerSplit.Splits;
        Span<char> timesBuffer = stackalloc char[512];
        int timesBufferLength = StringHelper.Join(splits[timeStartIndex..], ' ', timesBuffer);
        string times = new(timesBuffer[..timesBufferLength]);

        string[] matches = _timeConversions.Select(t => t.Regex.Matches(times).Select(static m => m.Value)).SelectMany(static m => m).ToArray();
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

    [SkipLocalsInit]
    private string[] GetTargets()
    {
        ReadOnlySpan<char> match = _targetPattern.Match(ChatMessage.Message).Value;
        int firstWordLength = match.IndexOf(' ');
        match = match[firstWordLength..];

        Span<Range> ranges = stackalloc Range[256];
        int rangesLength = match.Split(ranges, ',');
        ranges = ranges[..rangesLength];

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
        IEnumerable<string> pattern = typeof(RemindCommand).GetFields(BindingFlags.Static | BindingFlags.NonPublic).Where(static f => f.GetCustomAttribute<TimePatternAttribute>() is not null).Select(static f => f.GetValue(null)!.ToString()![2..^2]);
        return '(' + string.Join(")|(", pattern) + ')';
    }

    private static TimeConversionMethod[] GetTimeConversionMethods()
    {
        FieldInfo[] fields = typeof(RemindCommand).GetFields(BindingFlags.Static | BindingFlags.NonPublic).Where(static f => f.GetCustomAttribute<TimePatternAttribute>() is not null).ToArray();
        string[] methodNames = fields.Select(static f => f.GetCustomAttribute<TimePatternAttribute>()!.ConversionMethod).ToArray();
        MethodInfo[] methods = typeof(TimeSpan).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(f => methodNames.Contains(f.Name)).ToArray();
        return fields.Select(f =>
        {
            TimePatternAttribute attr = f.GetCustomAttribute<TimePatternAttribute>()!;
            Regex regex = (Regex)f.GetValue(null)!;
            MethodInfo method = methods.First(m => m.Name == attr.ConversionMethod);
            return new TimeConversionMethod(regex, method.CreateDelegate<Func<double, TimeSpan>>(), attr.Factor);
        }).ToArray();
    }

    private static unsafe int GetTimeStartIndex(ChatMessageExtension* messageExtension)
    {
        SmartSplit lowerSplit = messageExtension->LowerSplit;
        for (int i = 2; i < lowerSplit.Length; i++)
        {
            if (lowerSplit[i].Span is _timeIdentificator)
            {
                return i + 1;
            }
        }

        return -1;
    }

    public void Dispose() => Response.Dispose();

    public bool Equals(RemindCommand other) =>
        _twitchBot.Equals(other._twitchBot) && _prefix.Equals(other._prefix) && _alias.Equals(other._alias) &&
        Response.Equals(other.Response) && ChatMessage.Equals(other.ChatMessage);

    public override bool Equals(object? obj) => obj is RemindCommand other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_twitchBot, _prefix, _alias, Response, ChatMessage);

    public static bool operator ==(RemindCommand left, RemindCommand right) => left.Equals(right);

    public static bool operator !=(RemindCommand left, RemindCommand right) => !left.Equals(right);
}
