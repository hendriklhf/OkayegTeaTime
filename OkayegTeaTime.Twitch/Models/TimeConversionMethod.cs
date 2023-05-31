using System;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Twitch.Models;

public sealed class TimeConversionMethod
{
    public Regex Regex { get; }

    public Func<double, TimeSpan> Method { get; }

    public int Factor { get; }

    public TimeConversionMethod(Regex regex, Func<double, TimeSpan> method, int factor)
    {
        Regex = regex;
        Method = method;
        Factor = factor;
    }
}
