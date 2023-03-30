using System;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ExecutionMethodAttribute : Attribute
{
    public CommandType CommandType { get; }

    public ExecutionMethodAttribute(CommandType commandType)
    {
        CommandType = commandType;
    }
}
