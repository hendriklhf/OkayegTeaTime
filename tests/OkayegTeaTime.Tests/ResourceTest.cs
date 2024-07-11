using System.Reflection;
using OkayegTeaTime.Resources;
using Xunit;

namespace OkayegTeaTime.Tests;

public sealed class ResourceTest
{
    [Fact]
    public void AllPropertiesNotNullOrEmptyTest()
    {
        PropertyInfo[] properties = typeof(ResourceController).GetProperties(BindingFlags.Static | BindingFlags.Public);
        Assert.NotEmpty(properties);
        foreach (PropertyInfo prop in properties)
        {
            string? value = (string?)prop.GetValue(null);
            Assert.True(!string.IsNullOrWhiteSpace(value));
        }
    }
}
