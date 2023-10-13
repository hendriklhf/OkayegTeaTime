using System;
using JetBrains.Annotations;

namespace OkayegTeaTime.Twitch.Attributes;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Field)]
public sealed class TimePatternAttribute(string conversionMethod, int factor = 1) : Attribute
{
    public string ConversionMethod { get; } = conversionMethod;

    public int Factor { get; } = factor;
}
