using System;
using JetBrains.Annotations;
using OkayegTeaTime.Twitch.Commands;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Attributes;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class HandledCommandAttribute<TCommand>(CommandType commandType) :
    HandledCommandAttribute(typeof(TCommand), commandType)
    where TCommand : IChatCommand<TCommand>;
