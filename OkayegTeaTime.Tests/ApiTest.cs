using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime.Files.Jsons.HttpRequests.Bttv;
using OkayegTeaTime.Files.Jsons.HttpRequests.Ffz;
using OkayegTeaTime.Files.Jsons.HttpRequests.SevenTv;
using OkayegTeaTime.Twitch.Controller;

namespace OkayegTeaTime.Tests;

[TestClass]
public class ApiTest
{
    private const long _testChannel = 87633910;

    [TestMethod]
    public void GetSevenTvEmotesTest()
    {
        SevenTvEmote[] emotes = new EmoteController().GetSevenTvEmotes(_testChannel).ToArray();
        Assert.IsTrue(emotes.Any());
    }

    [TestMethod]
    public void GetBttvEmotesTest()
    {
        BttvEmote[] emotes = new EmoteController().GetBttvEmotes(_testChannel).ToArray();
        Assert.IsTrue(emotes.Any());
    }

    [TestMethod]
    public void GetFfzEmotesTest()
    {
        FfzEmote[] emotes = new EmoteController().GetFfzEmotes(_testChannel).ToArray();
        Assert.IsTrue(emotes.Any());
    }

    [TestMethod]
    public void GetSevenTvGlobalEmotes()
    {
        SevenTvGlobalEmote[] emotes = new EmoteController().SevenTvGlobalEmotes.ToArray();
        Assert.IsTrue(emotes.Any());
    }


    [TestMethod]
    public void GetBttvGlobalEmotesTest()
    {
        BttvEmote[] emotes = new EmoteController().BttvGlobalEmotes.ToArray();
        Assert.IsTrue(emotes.Any());
    }

    [TestMethod]
    public void GetFfzGlobalEmotesTest()
    {
        FfzEmote[] emotes = new EmoteController().FfzGlobalEmotes.ToArray();
        Assert.IsTrue(emotes.Any());
    }
}
