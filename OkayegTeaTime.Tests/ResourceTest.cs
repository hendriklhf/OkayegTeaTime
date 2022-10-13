using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime.Resources;

namespace OkayegTeaTime.Tests;

[TestClass]
public sealed class ResourceTest
{
    [TestMethod]
    public void AllPropertiesNotNullOrEmptyTest()
    {
        PropertyInfo[] properties = typeof(ResourceController).GetProperties(BindingFlags.Static | BindingFlags.Public);
        Assert.IsTrue(properties.Length > 0);
        foreach (PropertyInfo prop in properties)
        {
            string? value = (string?)prop.GetValue(null);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(value));
        }
    }
}
