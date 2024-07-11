using System;
using System.Linq;
using System.Reflection;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;
using Xunit;

namespace OkayegTeaTime.Tests;

public sealed class CommandTest
{
    private readonly CommandType[] _commandTypes = Enum.GetValues<CommandType>();
    private readonly AfkType[] _afkTypes = Enum.GetValues<AfkType>();

    public CommandTest() => GlobalSettings.Initialize();

    [Fact]
    public void CommandCompletenessTestFromEnum()
    {
        foreach (CommandType type in _commandTypes)
        {
            Command command = CommandController.GetCommand(type);
            Assert.Equal(type, command.Type);
            Assert.NotNull(command);
        }
    }

    [Fact]
    public void CommandCompletenessTestFromJson()
    {
        foreach (Command command in CommandController.Commands)
        {
            CommandType type = Enum.Parse<CommandType>(command.Name, true);
            Assert.Equal(command.Type, type);
        }
    }

    [Fact]
    public void CommandCompletenessFromHandledCommands()
    {
        HandledCommandAttribute[]? handles = Assembly.GetAssembly(typeof(HandledCommandAttribute))?.GetTypes()
            .Where(static t => t.GetCustomAttribute<HandledCommandAttribute>() is not null)
            .Select(static t => t.GetCustomAttribute<HandledCommandAttribute>()!)
            .ToArray();
        Assert.NotNull(handles);
        Assert.True(handles.Length <= _commandTypes.Length);

        foreach (HandledCommandAttribute handle in handles)
        {
            Command command = CommandController.GetCommand(handle.CommandType);
            Assert.NotNull(command);
            Assert.Equal(handle.CommandType, command.Type);
            Assert.Contains(handle.CommandType, _commandTypes);
        }
    }

    [Fact]
    public void CommandAccessibleByEnumIndexTest()
    {
        foreach (CommandType type in _commandTypes)
        {
            Command command = CommandController.Commands[(int)type];
            CommandType parsedType = Enum.Parse<CommandType>(command.Name, true);
            Assert.Equal(type, parsedType);
        }
    }

    [Fact]
    public void AfkCommandCompletenessTestFromEnum()
    {
        foreach (AfkType type in _afkTypes)
        {
            AfkCommand command = CommandController.GetAfkCommand(type);
            Assert.NotNull(command);
        }
    }

    [Fact]
    public void AfkCommandCompletenessTestFromJson()
    {
        foreach (AfkCommand command in CommandController.AfkCommands)
        {
            AfkType type = Enum.Parse<AfkType>(command.Name, true);
            Assert.NotNull(type);
        }
    }

    [Fact]
    public void AfkCommandAccessibleByEnumIndexTest()
    {
        foreach (AfkType type in _afkTypes)
        {
            AfkCommand command = CommandController.AfkCommands[(int)type];
            AfkType parsedType = Enum.Parse<AfkType>(command.Name, true);
            Assert.Equal(type, parsedType);
        }
    }
}
