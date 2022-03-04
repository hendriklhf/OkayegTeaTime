using System;
using System.Linq;
using HLE.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.JsonClasses.CommandData;
using OkayegTeaTime.Twitch.Commands.Enums;
using JCommand = OkayegTeaTime.Files.JsonClasses.CommandData.Command;

namespace OkayegTeaTime.Tests;

[TestClass]
public class CommandTests
{
    [TestMethod]
    public void CommandCompletenessTestFromEnum()
    {
        JsonController.Initialize();
        ((CommandType[])Enum.GetValues(typeof(CommandType))).ForEach(type =>
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
            CommandType type = ((CommandType[])Enum.GetValues(typeof(CommandType))).SingleOrDefault(c => c.ToString().ToLower() == cmd.Name.ToLower());
            Assert.IsNotNull(type);
        });
    }

    [TestMethod]
    public void AfkCommandCompletenessTestFromEnum()
    {
        JsonController.Initialize();
        ((AfkCommandType[])Enum.GetValues(typeof(AfkCommandType))).ForEach(type =>
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
            AfkCommandType type = ((AfkCommandType[])Enum.GetValues(typeof(AfkCommandType))).SingleOrDefault(c => c.ToString().ToLower() == cmd.Name);
            Assert.IsNotNull(type);
        });
    }
}
