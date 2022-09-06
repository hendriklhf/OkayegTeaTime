using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HLE.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;
using JCommand = OkayegTeaTime.Files.Models.Command;

namespace OkayegTeaTime.Tests;

[TestClass]
public class CommandTest
{
    private readonly CommandType[] _commandTypes = Enum.GetValues<CommandType>();
    private readonly AfkType[] _afkTypes = Enum.GetValues<AfkType>();
    private readonly CommandController _commandController = new();

    [TestMethod]
    public void CommandCompletenessTestFromEnum()
    {
        foreach (CommandType type in _commandTypes)
        {
            JCommand command = _commandController[type];
            Assert.IsNotNull(command);
        }
    }

    [TestMethod]
    public void CommandCompletenessTestFromJson()
    {
        _commandController.Commands.ForEach(cmd =>
        {
            CommandType type = _commandTypes.SingleOrDefault(c => string.Equals(c.ToString(), cmd.Name, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(type);
        });
    }

    [TestMethod]
    public void CommandCompletenessFromHandledCommands()
    {
        HandledCommand[]? handles = Assembly.GetAssembly(typeof(HandledCommand))?.GetTypes()
            .Where(t => t.GetCustomAttribute<HandledCommand>() is not null)
            .Select(t => t.GetCustomAttribute<HandledCommand>()!).ToArray();
        Assert.IsNotNull(handles);
        Assert.IsTrue(handles.Length == _commandTypes.Length);

        foreach (HandledCommand handle in handles)
        {
            Assert.IsNotNull(_commandController[handle.CommandType]);
            Assert.IsTrue(_commandTypes.Contains(handle.CommandType));
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
        _commandController.AfkCommands.ForEach(cmd =>
        {
            AfkType type = _afkTypes.SingleOrDefault(c => string.Equals(c.ToString(), cmd.Name, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(type);
        });
    }
}
