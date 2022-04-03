using HLE.Collections;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Api;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

public class RemindCommand : Command
{
    private static readonly Regex _targetPattern = new($@"^\S+\s{Pattern.MultipleReminderTargets}", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _timeSplitPattern = new($@"\b{Pattern.TimeSplit}\b", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _exceptMessagePattern = new($@"{_targetPattern}(\sin(\s({Pattern.TimeSplit}))+)?", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

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
        Regex nextMessagePattern = PatternCreator.Create(Alias, Prefix, @"\s\w+(\s\S+)*");
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
            bool multi = ids.Count(i => i != default) > 1;
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
        List<string> captures = matches.Select(m => m.Value).ToList();
        return TimeHelper.ConvertTimeToMilliseconds(captures) + TimeHelper.Now();
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
}
