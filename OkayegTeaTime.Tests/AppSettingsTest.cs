using System.Reflection;
using HLE.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime.Settings;

namespace OkayegTeaTime.Tests;

[TestClass]
public sealed class AppSettingsTest
{
    public AppSettingsTest()
    {
        AppSettings.Initialize();
    }

    [TestMethod]
    public void AllPropertiesNotDefaultTest()
    {
        PropertyInfo[] properties = typeof(AppSettings).GetProperties();
        Assert.IsTrue(properties.Length > 0);
        properties.ForEach(p => Assert.IsTrue(p.GetValue(null, null) != default));
    }
}
