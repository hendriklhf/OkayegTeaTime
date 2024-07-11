using System;
using HLE.Strings;

namespace OkayegTeaTime.Utils;

public static class TimeSpanFormatter
{
    private const string SpanFormatDefault = "<1s";

    public static int Format(TimeSpan timeSpan, Span<char> buffer)
    {
        ValueStringBuilder builder = new(buffer);
        if (timeSpan.Days != 0)
        {
            builder.Append(timeSpan.Days);
            builder.Append('d');
        }

        if (timeSpan.Hours != 0)
        {
            if (builder.Length != 0)
            {
                builder.Append(", ");
            }

            builder.Append(timeSpan.Hours);
            builder.Append('h');
        }

        if (timeSpan.Minutes != 0)
        {
            if (builder.Length != 0)
            {
                builder.Append(", ");
            }

            builder.Append(timeSpan.Minutes);
            builder.Append("min");
        }

        if (timeSpan.Seconds != 0)
        {
            if (builder.Length != 0)
            {
                builder.Append(", ");
            }

            builder.Append(timeSpan.Seconds);
            builder.Append('s');
        }

        if (builder.Length == 0)
        {
            builder.Append(SpanFormatDefault);
        }

        return builder.Length;
    }

    public static string Format(TimeSpan timeSpan)
    {
        Span<char> buffer = stackalloc char[100];
        int length = Format(timeSpan, buffer);
        return new(buffer[..length]);
    }
}
