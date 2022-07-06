using System.Reflection;
using HLE.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime.Files;

namespace OkayegTeaTime.Tests;

[TestClass]
public class AppSettingsTest
{
    [TestMethod]
    public void AllPropertiesNotDefaultTest()
    {
        PropertyInfo[] properties = typeof(AppSettings).GetProperties();
        properties.ForEach(p => Assert.IsTrue(p.GetValue(null, null) != default));
    }
}
