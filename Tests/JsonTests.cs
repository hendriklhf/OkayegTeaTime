using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.Settings;

namespace Tests
{
    [TestClass]
    public class JsonTests
    {
        [TestMethod]
        public void JsonBotDataContentTest()
        {
            JsonController jsonController = new();
            jsonController.LoadData();
            UserLists userLists = jsonController.BotData.UserLists;
            Assert.IsTrue(userLists.Moderators.Any());
            Assert.IsTrue(userLists.Owners.Any());
            Assert.IsTrue(userLists.SecretUsers.Any());
            Assert.IsTrue(userLists.SpecialUsers.Any());
        }

        [TestMethod]
        public void JsonCommmandsContentTest()
        {
            JsonController jsonController = new();
            jsonController.LoadData();
            Assert.IsTrue(jsonController.CommandLists.AfkCommands.Any());
            Assert.IsTrue(jsonController.CommandLists.Commands.Any());
        }
    }
}
