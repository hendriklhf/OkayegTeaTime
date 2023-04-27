using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Settings;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Tests;

[TestClass]
public sealed class CommandTest
{
    private readonly CommandType[] _commandTypes = Enum.GetValues<CommandType>();
    private readonly AfkType[] _afkTypes = Enum.GetValues<AfkType>();
    private readonly CommandController _commandController;

    public CommandTest()
    {
        AppSettings.Initialize();
        _commandController = new();
    }

    [TestMethod]
    public void CommandCompletenessTestFromEnum()
    {
        foreach (CommandType type in _commandTypes)
        {
            Command command = _commandController[type];
            Assert.IsNotNull(command);
        }
    }

    [TestMethod]
    public void CommandCompletenessTestFromJson()
    {
        foreach (Command command in _commandController.Commands)
        {
            CommandType type = Enum.Parse<CommandType>(command.Name, true);
            Assert.IsNotNull(type);
        }
    }

    [TestMethod]
    public void CommandCompletenessFromHandledCommands()
    {
        HandledCommandAttribute[]? handles = Assembly.GetAssembly(typeof(HandledCommandAttribute))?.GetTypes()
            .Where(t => t.GetCustomAttribute<HandledCommandAttribute>() is not null)
            .Select(t => t.GetCustomAttribute<HandledCommandAttribute>()!)
            .ToArray();
        Assert.IsNotNull(handles);
        Assert.IsTrue(handles.Length <= _commandTypes.Length);

        foreach (HandledCommandAttribute handle in handles)
        {
            Assert.IsNotNull(_commandController[handle.CommandType]);
            Assert.IsTrue(_commandTypes.Contains(handle.CommandType));
        }
    }

    [TestMethod]
    public void CommandAccessibleByEnumIndexTest()
    {
        foreach (CommandType type in _commandTypes)
        {
            Command command = _commandController.Commands[(int)type];
            CommandType parsedType = Enum.Parse<CommandType>(command.Name, true);
            Assert.AreEqual(type, parsedType);
        }
    }

    [TestMethod]
    public void AfkCommandCompletenessTestFromEnum()
    {
        foreach (AfkType type in _afkTypes)
        {
            AfkCommand command = _commandController[type];
            Assert.IsNotNull(command);
        }
    }

    [TestMethod]
    public void AfkCommandCompletenessTestFromJson()
    {
        foreach (AfkCommand command in _commandController.AfkCommands)
        {
            AfkType type = Enum.Parse<AfkType>(command.Name, true);
            Assert.IsNotNull(type);
        }
    }

    [TestMethod]
    public void AfkCommandAccessibleByEnumIndexTest()
    {
        foreach (AfkType type in _afkTypes)
        {
            AfkCommand command = _commandController.AfkCommands[(int)type];
            AfkType parsedType = Enum.Parse<AfkType>(command.Name, true);
            Assert.AreEqual(type, parsedType);
        }
    }
}
