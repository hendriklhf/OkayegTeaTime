using System;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Twitch.Models;

public sealed class TimeConversionMethod(Regex regex, Func<double, TimeSpan> method, int factor)
{
    public Regex Regex { get; } = regex;

    public Func<double, TimeSpan> Method { get; } = method;

    public int Factor { get; } = factor;
}
