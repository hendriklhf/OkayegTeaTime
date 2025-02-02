using System;
using System.Diagnostics.Contracts;
using HLE.Text;

namespace OkayegTeaTime.Utils;

public static class TimeSpanFormatter
{
    private const string SpanFormatDefault = "<1s";

    [Pure]
    public static string Format(TimeSpan timeSpan)
    {
        using PooledStringBuilder builder = new(32);
        Format(timeSpan, builder);
        return builder.ToString();
    }

    public static void Format(TimeSpan timeSpan, PooledStringBuilder builder)
    {
        int startingLength = builder.Length;
        if (timeSpan.Days != 0)
        {
            builder.Append(timeSpan.Days);
            builder.Append('d');
        }

        if (timeSpan.Hours != 0)
        {
            if (builder.Length != startingLength)
            {
                builder.Append(", ");
            }

            builder.Append(timeSpan.Hours);
            builder.Append('h');
        }

        if (timeSpan.Minutes != 0)
        {
            if (builder.Length != startingLength)
            {
                builder.Append(", ");
            }

            builder.Append(timeSpan.Minutes);
            builder.Append("min");
        }

        if (timeSpan.Seconds != 0)
        {
            if (builder.Length != startingLength)
            {
                builder.Append(", ");
            }

            builder.Append(timeSpan.Seconds);
            builder.Append('s');
        }

        if (builder.Length == startingLength)
        {
            builder.Append(SpanFormatDefault);
        }
    }
}
