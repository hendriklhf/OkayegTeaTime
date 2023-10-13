using System;
using JetBrains.Annotations;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Attributes;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class HandledCommandAttribute(CommandType commandType, Type command)
    : Attribute
{
    public CommandType CommandType { get; } = commandType;

    public Type Command { get; } = command;
}
