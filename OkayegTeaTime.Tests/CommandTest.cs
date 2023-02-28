using System;
using System.Linq;
using System.Reflection;
using HLE.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime.Database.Cache.Enums;
using OkayegTeaTime.Models.Json;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Controller;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Tests;

[TestClass]
public sealed class CommandTest
{
    private readonly CommandType[] _commandTypes = Enum.GetValues<CommandType>();
    private readonly AfkType[] _afkTypes = Enum.GetValues<AfkType>();
    private readonly CommandController _commandController = new(null);

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
        _commandController.Commands.ForEach(cmd =>
        {
            CommandType type = _commandTypes.SingleOrDefault(c => string.Equals(c.ToString(), cmd.Name, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(type);
        });
    }

    [TestMethod]
    public void CommandCompletenessFromHandledCommands()
    {
        HandledCommandAttribute[]? handles = Assembly.GetAssembly(typeof(HandledCommandAttribute))?.GetTypes().Where(t => t.GetCustomAttribute<HandledCommandAttribute>() is not null).Select(t => t.GetCustomAttribute<HandledCommandAttribute>()!)
            .ToArray();
        Assert.IsNotNull(handles);
        Assert.IsTrue(handles.Length == _commandTypes.Length);

        foreach (HandledCommandAttribute handle in handles)
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
