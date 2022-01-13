using System.Linq;
using HLE.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.JsonClasses.CommandData;
using OkayegTeaTime.Twitch.Commands.Enums;
using JCommand = OkayegTeaTime.Files.JsonClasses.CommandData.Command;

namespace Tests;

[TestClass]
public class CommandTests
{
    [TestMethod]
    public void CommandCompletenessTestFromEnum()
    {
        JsonController.Initialize();
        typeof(CommandType).ToList<CommandType>().ForEach(type =>
        {
            JCommand command = AppSettings.CommandList[type];
            Assert.IsNotNull(command);
        });
    }

    [TestMethod]
    public void CommandCompletenessTestFromJson()
    {
        JsonController.Initialize();
        AppSettings.CommandList.Commands.ForEach(cmd =>
        {
            CommandType type = typeof(CommandType).ToList<CommandType>().SingleOrDefault(c => c.ToString().ToLower() == cmd.CommandName.ToLower());
            Assert.IsNotNull(type);
        });
    }

    [TestMethod]
    public void AfkCommandCompletenessTestFromEnum()
    {
        JsonController.Initialize();
        typeof(AfkCommandType).ToList<AfkCommandType>().ForEach(type =>
        {
            AfkCommand command = AppSettings.CommandList[type];
            Assert.IsNotNull(command);
        });
    }

    [TestMethod]
    public void AfkCommandCompletenessTestFromJson()
    {
        JsonController.Initialize();
        AppSettings.CommandList.AfkCommands.ForEach(cmd =>
        {
            AfkCommandType type = typeof(AfkCommandType).ToList<AfkCommandType>().SingleOrDefault(c => c.ToString().ToLower() == cmd.CommandName);
            Assert.IsNotNull(type);
        });
    }
}
