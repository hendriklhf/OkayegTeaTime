using System.Reflection;
using HLE.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime.Files;

namespace OkayegTeaTime.Tests;

[TestClass]
public class JsonTest
{
    [TestMethod]
    public void JsonSettingsContentTest()
    {
        Assert.IsNotNull(AppSettings.UserLists.Moderators);
        Assert.IsTrue(AppSettings.UserLists.Owner != default);
        Assert.IsNotNull(AppSettings.UserLists.SecretUsers);
        Assert.IsNotNull(AppSettings.UserLists.IgnoredUsers);
    }

    [TestMethod]
    public void AppSettingsCompletePropertiesTest()
    {
        PropertyInfo[] properties = typeof(AppSettings).GetProperties();
        properties.ForEach(p => Assert.IsNotNull(p.GetValue(null, null)));
    }
}
