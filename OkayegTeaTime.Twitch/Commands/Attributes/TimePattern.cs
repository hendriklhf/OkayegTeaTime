using System;

namespace OkayegTeaTime.Twitch.Commands.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class TimePattern : Attribute
{
    public string ConversionMethod { get; }

    public int Factor { get; }

    public TimePattern(string conversionMethod, int factor = 1)
    {
        ConversionMethod = conversionMethod;
        Factor = factor;
    }
}
