using System;
using System.Text.RegularExpressions;

namespace OkayegTeaTime.Twitch.Models;

public sealed unsafe class TimeConversionMethod
{
    public Regex Regex { get; }

    public delegate*<double, TimeSpan> Method { get; }

    public int Factor { get; }

    public TimeConversionMethod(Regex regex, delegate*<double, TimeSpan> method, int factor)
    {
        Regex = regex;
        Method = method;
        Factor = factor;
    }
}
