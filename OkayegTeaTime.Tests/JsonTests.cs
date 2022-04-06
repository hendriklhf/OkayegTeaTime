using System.Reflection;
using HLE.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OkayegTeaTime.Tests;

[TestClass]
public class JsonTests
{
    [TestMethod]
    public void JsonSettingsContentTest()
    {
        Assert.IsTrue(AppSettings.UserLists.Moderators is not null);
        Assert.IsTrue(AppSettings.UserLists.Owners is not null);
        Assert.IsTrue(AppSettings.UserLists.SecretUsers is not null);
        Assert.IsTrue(AppSettings.UserLists.IgnoredUsers is not null);
    }

    [TestMethod]
    public void AppSettingsCompletePropertiesTest()
    {
        PropertyInfo[] properties = typeof(AppSettings).GetProperties();
        properties.ForEach(p =>
        {
            Assert.IsNotNull(p.GetValue(null, null));
        });
    }
}
