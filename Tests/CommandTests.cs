using System.Linq;
using HLE.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData;
using OkayegTeaTimeCSharp.Twitch.Commands;
using OkayegTeaTimeCSharp.Twitch.Commands.Enums;
using JCommand = OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData.Command;

namespace Tests;

[TestClass]
public class CommandTests
{
    [TestMethod]
    public void CommandCompletenessTestFromEnum()
    {
        JsonController jsonController = new();
        jsonController.LoadData();
        typeof(CommandType).ToList<CommandType>().ForEach(type =>
        {
            JCommand command = CommandHelper.GetCommand(type);
            Assert.IsNotNull(command);
        });
    }

    [TestMethod]
    public void CommandCompletenessTestFromJson()
    {
        JsonController jsonController = new();
        jsonController.LoadData();
        jsonController.CommandLists.Commands.ForEach(cmd =>
        {
            CommandType type = typeof(CommandType).ToList<CommandType>().SingleOrDefault(c => c.ToString().ToLower() == cmd.CommandName.ToLower());
            Assert.IsNotNull(type);
        });
    }

    [TestMethod]
    public void AfkCommandCompletenessTestFromEnum()
    {
        JsonController jsonController = new();
        jsonController.LoadData();
        typeof(AfkCommandType).ToList<AfkCommandType>().ForEach(type =>
        {
            AfkCommand command = CommandHelper.GetAfkCommand(type);
            Assert.IsNotNull(command);
        });
    }

    [TestMethod]
    public void AfkCommandCompletenessTestFromJson()
    {
        JsonController jsonController = new();
        jsonController.LoadData();
        jsonController.CommandLists.AfkCommands.ForEach(cmd =>
        {
            AfkCommandType type = typeof(AfkCommandType).ToList<AfkCommandType>().SingleOrDefault(c => c.ToString().ToLower() == cmd.CommandName);
            Assert.IsNotNull(type);
        });
    }
}
