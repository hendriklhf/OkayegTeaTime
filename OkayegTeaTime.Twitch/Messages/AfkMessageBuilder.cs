﻿using System;
using System.Diagnostics;
using System.Linq;
using HLE.Memory;
using HLE.Strings;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Messages;

public sealed class AfkMessageBuilder : IEquatable<AfkMessageBuilder>
{
    private readonly string[][] _comingBackMessageParts;
    private readonly string[][] _goingAwayMessageParts;
    private readonly string[][] _resumingMessageParts;

    public AfkMessageBuilder(ReadOnlySpan<AfkCommand> afkCommands)
    {
        int afkTypeCount = Enum.GetValues<AfkType>().Length;
        _comingBackMessageParts = GetComingBackMessageParts(afkCommands, afkTypeCount);
        _goingAwayMessageParts = GetGoingAwayMessageParts(afkCommands, afkTypeCount);
        _resumingMessageParts = GetResumingMessageParts(afkCommands, afkTypeCount);
    }

    public int BuildComingBackMessage(User user, AfkType afkType, Span<char> resultBuffer)
    {
        ValueStringBuilder builder = resultBuffer;
        ReadOnlySpan<string> messageParts = _comingBackMessageParts[(int)afkType];
        Debug.Assert(messageParts.Length == 3);

        TimeSpan afkDuration = DateTime.UtcNow - DateTimeOffset.FromUnixTimeMilliseconds(user.AfkTime);
        builder.Append(user.Username, messageParts[0], user.AfkMessage, messageParts[1]);
        int durationLength = afkDuration.Format(builder.FreeBuffer);
        builder.Advance(durationLength);
        builder.Append(" ago", messageParts[2]);
        return builder.Length;
    }

    public int BuildGoingAwayMessage(ReadOnlySpan<char> username, AfkType afkType, Span<char> resultBuffer)
    {
        ValueStringBuilder builder = resultBuffer;
        ReadOnlySpan<string> messageParts = _goingAwayMessageParts[(int)afkType];
        Debug.Assert(messageParts.Length == 1);

        builder.Append(username, messageParts[0]);
        return builder.Length;
    }

    public int BuildResumingMessage(ReadOnlySpan<char> username, AfkType afkType, Span<char> resultBuffer)
    {
        ValueStringBuilder builder = resultBuffer;
        ReadOnlySpan<string> messageParts = _resumingMessageParts[(int)afkType];
        Debug.Assert(messageParts.Length == 1);

        builder.Append(username, messageParts[0]);
        return builder.Length;
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

        Debug.Assert(resumingMessageParts.All(p => p.Length == 1), "all have one message part");
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

        Debug.Assert(goingAwayMessageParts.All(p => p.Length == 1), "all have one message part");
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

        Debug.Assert(comingBackMessageParts.All(p => p.Length == 3), "all have three message parts");
        return comingBackMessageParts;
    }

    public bool Equals(AfkMessageBuilder? other)
    {
        return ReferenceEquals(this, other);
    }

    public override bool Equals(object? obj)
    {
        return obj is AfkMessageBuilder other && Equals(other);
    }

    public override int GetHashCode()
    {
        return MemoryHelper.GetRawDataPointer(this).GetHashCode();
    }

    public static bool operator ==(AfkMessageBuilder? left, AfkMessageBuilder? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(AfkMessageBuilder? left, AfkMessageBuilder? right)
    {
        return !(left == right);
    }
}