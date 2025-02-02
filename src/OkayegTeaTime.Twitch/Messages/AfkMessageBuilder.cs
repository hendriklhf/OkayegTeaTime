using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using HLE;
using HLE.Text;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Messages;

public sealed class AfkMessageBuilder : IEquatable<AfkMessageBuilder>
{
    private readonly string[][] _comingBackMessageParts;
    private readonly string[][] _goingAwayMessageParts;
    private readonly string[][] _resumingMessageParts;

    public AfkMessageBuilder(ReadOnlySpan<AfkCommand> afkCommands)
    {
        int afkTypeCount = EnumValues<AfkType>.Count;
        _comingBackMessageParts = GetComingBackMessageParts(afkCommands, afkTypeCount);
        _goingAwayMessageParts = GetGoingAwayMessageParts(afkCommands, afkTypeCount);
        _resumingMessageParts = GetResumingMessageParts(afkCommands, afkTypeCount);
    }

    public void BuildComingBackMessage(User user, AfkType afkType, PooledStringBuilder builder)
    {
        ReadOnlySpan<string> messageParts = _comingBackMessageParts[(int)afkType];
        Debug.Assert(messageParts.Length == 3);

#pragma warning disable S6354
        TimeSpan afkDuration = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(user.AfkTime);
#pragma warning restore S6354

#pragma warning disable RCS1217 // makes no sense
        builder.Append($"{user.Username}{messageParts[0]}{user.AfkMessage}{messageParts[1]}");
#pragma warning restore RCS1217

        TimeSpanFormatter.Format(afkDuration, builder);
        builder.Append(" ago");
        builder.Append(messageParts[2]);
    }

    public void BuildGoingAwayMessage(ReadOnlySpan<char> username, AfkType afkType, PooledStringBuilder builder)
    {
        ReadOnlySpan<string> messageParts = _goingAwayMessageParts[(int)afkType];
        Debug.Assert(messageParts.Length == 1);

        builder.Append(username);
        builder.Append(messageParts[0]);
    }

    public void BuildResumingMessage(ReadOnlySpan<char> username, AfkType afkType, PooledStringBuilder builder)
    {
        ReadOnlySpan<string> messageParts = _resumingMessageParts[(int)afkType];
        Debug.Assert(messageParts.Length == 1);

        builder.Append(username);
        builder.Append(messageParts[0]);
    }

    private static string[][] GetResumingMessageParts(ReadOnlySpan<AfkCommand> afkCommands, int afkTypeCount)
    {
        string[][] resumingMessageParts = new string[afkTypeCount][];
        foreach (AfkCommand afkCommand in afkCommands)
        {
            AfkType type = Enum.Parse<AfkType>(afkCommand.Name, true);
            string[] messageParts = afkCommand.Resuming.Split("{}", StringSplitOptions.RemoveEmptyEntries);
            resumingMessageParts[(int)type] = messageParts;
        }

        Debug.Assert(Array.TrueForAll(resumingMessageParts, static p => p.Length == 1), "all have one message part");
        return resumingMessageParts;
    }

    private static string[][] GetGoingAwayMessageParts(ReadOnlySpan<AfkCommand> afkCommands, int afkTypeCount)
    {
        string[][] goingAwayMessageParts = new string[afkTypeCount][];
        foreach (AfkCommand afkCommand in afkCommands)
        {
            AfkType type = Enum.Parse<AfkType>(afkCommand.Name, true);
            string[] messageParts = afkCommand.GoingAway.Split("{}", StringSplitOptions.RemoveEmptyEntries);
            goingAwayMessageParts[(int)type] = messageParts;
        }

        Debug.Assert(Array.TrueForAll(goingAwayMessageParts, static p => p.Length == 1), "all have one message part");
        return goingAwayMessageParts;
    }

    private static string[][] GetComingBackMessageParts(ReadOnlySpan<AfkCommand> afkCommands, int afkTypeCount)
    {
        string[][] comingBackMessageParts = new string[afkTypeCount][];
        foreach (AfkCommand afkCommand in afkCommands)
        {
            AfkType type = Enum.Parse<AfkType>(afkCommand.Name, true);
            string[] messageParts = afkCommand.ComingBack.Split("{}", StringSplitOptions.RemoveEmptyEntries);
            comingBackMessageParts[(int)type] = messageParts;
        }

        Debug.Assert(Array.TrueForAll(comingBackMessageParts, static p => p.Length == 3), "all have three message parts");
        return comingBackMessageParts;
    }

    public bool Equals(AfkMessageBuilder? other) => ReferenceEquals(this, other);

    public override bool Equals(object? obj) => obj is AfkMessageBuilder other && Equals(other);

    public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

    public static bool operator ==(AfkMessageBuilder? left, AfkMessageBuilder? right) => Equals(left, right);

    public static bool operator !=(AfkMessageBuilder? left, AfkMessageBuilder? right) => !(left == right);
}
