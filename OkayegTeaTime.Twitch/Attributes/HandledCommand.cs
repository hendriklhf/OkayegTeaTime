using System;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class HandledCommand : Attribute
{
    public CommandType CommandType { get; }

    public HandledCommand(CommandType commandType)
    {
        CommandType = commandType;
    }
}
