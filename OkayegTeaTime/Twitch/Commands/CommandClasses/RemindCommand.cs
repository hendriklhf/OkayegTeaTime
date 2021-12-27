using System.Text.RegularExpressions;
using HLE.Collections;
using HLE.Strings;
using HLE.Time;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Messages.Interfaces;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class RemindCommand : Command
{
    private static readonly Regex _targetPattern = new($@"^\S+\s{Pattern.MultipleReminderTargets}", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _timeSplitPattern = new(Pattern.TimeSplit, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));
    private static readonly Regex _exceptMessagePattern = new($@"{_targetPattern}\s(in\s({Pattern.TimeSplit}\s)+)?", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    public RemindCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex timedPattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, Pattern.ReminderInTime);
        if (timedPattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetReminder(ChatMessage, GetTargets(), GetMessage(), GetToTime()));
            return;
        }

        Regex nextMessagePattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+(\s\S+)*");
        if (nextMessagePattern.IsMatch(ChatMessage.Message))
        {
            TwitchBot.Send(ChatMessage.Channel, BotActions.SendSetReminder(ChatMessage, GetTargets(), GetMessage()));
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
