using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.Jsons.CommandData;
using OkayegTeaTime.Twitch.Commands.Enums;
using JCommand = OkayegTeaTime.Files.Jsons.CommandData.Command;

namespace OkayegTeaTime.Tests;

[TestClass]
public class CommandTests
{
    private readonly CommandType[] _commandTypes = Enum.GetValues<CommandType>();
    private readonly AfkCommandType[] _afkTypes = Enum.GetValues<AfkCommandType>();

    [TestMethod]
    public void CommandCompletenessTestFromEnum()
    {
        JsonController.Initialize();
        foreach (CommandType type in _commandTypes)
        {
            JCommand command = AppSettings.CommandList[type];
            Assert.IsNotNull(command);
        }
    }

    [TestMethod]
    public void CommandCompletenessTestFromJson()
    {
        JsonController.Initialize();
        AppSettings.CommandList.Commands.ForEach(cmd =>
        {
            CommandType type = _commandTypes.SingleOrDefault(c => string.Equals(c.ToString(), cmd.Name, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(type);
        });
    }

    [TestMethod]
    public void AfkCommandCompletenessTestFromEnum()
    {
        JsonController.Initialize();
        foreach (AfkCommandType type in _afkTypes)
        {
            AfkCommand command = AppSettings.CommandList[type];
            Assert.IsNotNull(command);
        }
    }

    [TestMethod]
    public void AfkCommandCompletenessTestFromJson()
    {
        JsonController.Initialize();
        AppSettings.CommandList.AfkCommands.ForEach(cmd =>
        {
            AfkCommandType type = _afkTypes.SingleOrDefault(c => string.Equals(c.ToString(), cmd.Name, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(type);
        });
    }
}
