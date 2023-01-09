using System;
using JetBrains.Annotations;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Attributes;

[AttributeUsage(AttributeTargets.Struct)]
[MeansImplicitUse]
public sealed class HandledCommandAttribute : Attribute
{
    public CommandType CommandType { get; }

    public HandledCommandAttribute(CommandType commandType)
    {
        CommandType = commandType;
    }
}
