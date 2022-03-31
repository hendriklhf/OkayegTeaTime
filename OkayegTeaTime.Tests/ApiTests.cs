using System.Collections.Generic;
using System.Linq;
using HLE.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTime.Files.Jsons.HttpRequests.Bttv;
using OkayegTeaTime.Files.Jsons.HttpRequests.Ffz;
using OkayegTeaTime.Files.Jsons.HttpRequests.SevenTv;
using OkayegTeaTime.HttpRequests;
using OkayegTeaTime.Twitch.Api;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Tests;

[TestClass]
public class ApiTests
{
    private const string _testChannel = "strbhlfe";

    [TestMethod]
    public void GetSevenTvEmotesTest()
    {
        int emoteCount = 5;
        // FIXME: making HTTP call from tests. Should be mocked out to remove external dependency
        List<SevenTvEmote> emotes = HttpRequest.GetSevenTvEmotes(_testChannel, emoteCount)?.ToList();
        Assert.IsTrue(emotes?.Count == emoteCount);
        emotes.ForEach(e =>
        {
            bool isMatch = e.Name.IsMatch(@"^\w+$");
            Assert.IsTrue(isMatch);
        });
    }

    [TestMethod]
    public void GetBttvEmotesTest()
    {
        TwitchApi.Initialize();
        int emoteCount = 5;
        List<BttvSharedEmote> emotes = HttpRequest.GetBttvEmotes(_testChannel, emoteCount)?.ToList();
        Assert.IsTrue(emotes?.Count == emoteCount);
        emotes.ForEach(e =>
        {
            bool isMatch = e.Name.IsMatch(@"^\w+$");
            Assert.IsTrue(isMatch);
        });
    }

    [TestMethod]
    public void GetChatterCountTest()
    {
        int chatterCount = HttpRequest.GetChatterCount(_testChannel);
        Assert.IsNotNull(chatterCount);
    }

    [TestMethod]
    public void GetChattersTest()
    {
        List<Chatter> chatters = HttpRequest.GetChatters(_testChannel).ToList();
        chatters.ForEach(c =>
        {
            bool isMatch = c.Username.IsMatch(@"^\w+$");
            Assert.IsTrue(isMatch);
        });
    }

    [TestMethod]
    public void GetFfzEmotesTest()
    {
        int emoteCount = 5;
        List<FfzEmote> emotes = HttpRequest.GetFfzEmotes(_testChannel, emoteCount)?.ToList();
        Assert.IsTrue(emotes?.Count == emoteCount);
        emotes.ForEach(e =>
        {
            bool isMatch = e.Name.IsMatch(@"^\w+$");
            Assert.IsTrue(isMatch);
        });
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
