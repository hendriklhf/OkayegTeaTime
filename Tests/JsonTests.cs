using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.Settings;

namespace Tests;

[TestClass]
public class JsonTests
{
    [TestMethod]
    public void JsonSettingsContentTest()
    {
        JsonController jsonController = new();
        jsonController.LoadData();
        UserLists userLists = jsonController.Settings.UserLists;
        Assert.IsTrue(userLists.Moderators is not null);
        Assert.IsTrue(userLists.Owners is not null);
        Assert.IsTrue(userLists.SecretUsers is not null);
        Assert.IsTrue(userLists.IgnoredUsers is not null);
    }

    [TestMethod]
    public void JsonCommmandsContentTest()
    {
        JsonController jsonController = new();
        jsonController.LoadData();
        Assert.IsTrue(jsonController.CommandLists.AfkCommands is not null);
        Assert.IsTrue(jsonController.CommandLists.Commands is not null);
    }
}
