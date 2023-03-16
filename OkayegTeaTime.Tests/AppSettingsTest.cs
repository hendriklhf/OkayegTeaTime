using System.Reflection;
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
        foreach (PropertyInfo property in properties)
        {
            Assert.IsTrue(property.GetValue(null) != default);
        }
    }
}
