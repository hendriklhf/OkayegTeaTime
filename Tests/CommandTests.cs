using System.Linq;
using HLE.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTimeCSharp.Files;
using OkayegTeaTimeCSharp.Files.JsonClasses.CommandData;
using OkayegTeaTimeCSharp.Twitch.Commands.Enums;
using JCommand = OkayegTeaTimeCSharp.Files.JsonClasses.CommandData.Command;

namespace Tests;

[TestClass]
public class CommandTests
{
    [TestMethod]
    public void CommandCompletenessTestFromEnum()
    {
        JsonController.LoadJsonData();
        typeof(CommandType).ToList<CommandType>().ForEach(type =>
        {
            JCommand command = JsonController.CommandList[type];
            Assert.IsNotNull(command);
        });
    }

    [TestMethod]
    public void CommandCompletenessTestFromJson()
    {
        JsonController.LoadJsonData();
        JsonController.CommandList.Commands.ForEach(cmd =>
        {
            CommandType type = typeof(CommandType).ToList<CommandType>().SingleOrDefault(c => c.ToString().ToLower() == cmd.CommandName.ToLower());
            Assert.IsNotNull(type);
        });
    }

    [TestMethod]
    public void AfkCommandCompletenessTestFromEnum()
    {
        JsonController.LoadJsonData();
        typeof(AfkCommandType).ToList<AfkCommandType>().ForEach(type =>
        {
            AfkCommand command = JsonController.CommandList[type];
            Assert.IsNotNull(command);
        });
    }

    [TestMethod]
    public void AfkCommandCompletenessTestFromJson()
    {
        JsonController.LoadJsonData();
        JsonController.CommandList.AfkCommands.ForEach(cmd =>
        {
            AfkCommandType type = typeof(AfkCommandType).ToList<AfkCommandType>().SingleOrDefault(c => c.ToString().ToLower() == cmd.CommandName);
            Assert.IsNotNull(type);
        });
    }
}
