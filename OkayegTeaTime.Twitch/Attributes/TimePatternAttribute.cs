using System;

namespace OkayegTeaTime.Twitch.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public sealed class TimePatternAttribute : Attribute
{
    public string ConversionMethod { get; }

    public int Factor { get; }

    public TimePatternAttribute(string conversionMethod, int factor = 1)
    {
        ConversionMethod = conversionMethod;
        Factor = factor;
    }
}
