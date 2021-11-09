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
        JsonController.LoadJsonData();
        UserLists userLists = JsonController.Settings.UserLists;
        Assert.IsTrue(userLists.Moderators is not null);
        Assert.IsTrue(userLists.Owners is not null);
        Assert.IsTrue(userLists.SecretUsers is not null);
        Assert.IsTrue(userLists.IgnoredUsers is not null);
    }

    [TestMethod]
    public void JsonCommmandsContentTest()
    {
        JsonController.LoadJsonData();
        Assert.IsTrue(JsonController.CommandLists.AfkCommands is not null);
        Assert.IsTrue(JsonController.CommandLists.Commands is not null);
    }
}
