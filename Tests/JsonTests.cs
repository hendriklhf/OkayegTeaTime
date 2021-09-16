using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.Data;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class JsonTests
    {
        private JsonController _jsonController;

        [TestMethod]
        public void JsonControllerInitilizationTest()
        {
            _jsonController = new();
            Assert.IsNotNull(_jsonController);
        }

        [TestMethod]
        public void JsonBotDataContentTest()
        {
            UserLists userLists = _jsonController.BotData.UserLists;
            Assert.IsTrue(userLists.Moderators.Any());
            Assert.IsTrue(userLists.Owners.Any());
            Assert.IsTrue(userLists.SecretUsers.Any());
            Assert.IsTrue(userLists.SpecialUsers.Any());
        }

        public void JsonCommmandsContentTest()
        {
            Assert.IsTrue(_jsonController.CommandLists.AfkCommands.Any());
            Assert.IsTrue(_jsonController.CommandLists.Commands.Any());
        }
    }
}
