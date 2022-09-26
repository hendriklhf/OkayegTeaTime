using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime.Twitch.Controller;

namespace OkayegTeaTime.Tests;

[TestClass]
public sealed class ApiTest
{
    private const long _testChannel = 87633910;

    [TestMethod]
    public void GetSevenTvEmotesTest()
    {
        Assert.IsTrue(new EmoteController().GetSevenTvEmotes(_testChannel).Any());
    }

    [TestMethod]
    public void GetBttvEmotesTest()
    {
        Assert.IsTrue(new EmoteController().GetBttvEmotes(_testChannel).Any());
    }

    [TestMethod]
    public void GetFfzEmotesTest()
    {
        Assert.IsTrue(new EmoteController().GetFfzEmotes(_testChannel).Any());
    }

    [TestMethod]
    public void GetSevenTvGlobalEmotes()
    {
        Assert.IsTrue(new EmoteController().SevenTvGlobalEmotes.Any());
    }

    [TestMethod]
    public void GetBttvGlobalEmotesTest()
    {
        Assert.IsTrue(new EmoteController().BttvGlobalEmotes.Any());
    }

    [TestMethod]
    public void GetFfzGlobalEmotesTest()
    {
        Assert.IsTrue(new EmoteController().FfzGlobalEmotes.Any());
    }
}
