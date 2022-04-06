using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime.Files.Jsons.HttpRequests.Bttv;
using OkayegTeaTime.Files.Jsons.HttpRequests.Ffz;
using OkayegTeaTime.Files.Jsons.HttpRequests.SevenTv;
using OkayegTeaTime.HttpRequests;
using OkayegTeaTime.Twitch.Controller;

namespace OkayegTeaTime.Tests;

[TestClass]
public class ApiTests
{
    private const int _testChannel = 87633910;

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

    [TestMethod]
    public void GetMathResultTest()
    {
        string result = HttpRequest.GetMathResult("2^8");
        Assert.AreEqual("256", result);
    }

    [TestMethod]
    public void GetCSharpOnlineCompilerResultTest()
    {
        string result = HttpRequest.GetCSharpOnlineCompilerResult("print(\"test\");");
        Assert.AreEqual("test ", result);
    }

    [TestMethod]
    public void GetGoLangOnlineCompilerResultTest()
    {
        string result = HttpRequest.GetGoLangOnlineCompilerResult("fmt.Println(\"test\");");
        Assert.AreEqual("test\n", result);
    }

    [TestMethod]
    public void GetCppOnlineCompilerResultTest()
    {
        string result = HttpRequest.GetCppOnlineCompilerResult("std::cout << \"test\";");
        Assert.AreEqual("test", result);
    }
}
