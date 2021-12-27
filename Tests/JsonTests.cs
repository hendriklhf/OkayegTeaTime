using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime;
using OkayegTeaTime.Files;

namespace Tests;

[TestClass]
public class JsonTests
{
    [TestMethod]
    public void JsonSettingsContentTest()
    {
        JsonController.Initialize();
        Assert.IsTrue(AppSettings.UserLists.Moderators is not null);
        Assert.IsTrue(AppSettings.UserLists.Owners is not null);
        Assert.IsTrue(AppSettings.UserLists.SecretUsers is not null);
        Assert.IsTrue(AppSettings.UserLists.IgnoredUsers is not null);
    }

    [TestMethod]
    public void JsonCommmandsContentTest()
    {
        JsonController.Initialize();
        Assert.IsTrue(AppSettings.CommandList.AfkCommands is not null);
        Assert.IsTrue(AppSettings.CommandList.Commands is not null);
    }
}
