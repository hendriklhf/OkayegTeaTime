using System;
using JetBrains.Annotations;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Attributes;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Struct)]
public sealed class HandledCommandAttribute : Attribute
{
    public CommandType CommandType { get; }

    public Type Command { get; }

    public HandledCommandAttribute(CommandType commandType, Type command)
    {
        CommandType = commandType;
        Command = command;
    }
}
