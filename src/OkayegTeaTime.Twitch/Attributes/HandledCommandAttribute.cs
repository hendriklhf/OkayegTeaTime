using System;
using JetBrains.Annotations;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Attributes;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public abstract class HandledCommandAttribute(Type command, CommandType commandType) : Attribute
{
    public Type Command { get; } = command;

    public CommandType CommandType { get; } = commandType;
}
