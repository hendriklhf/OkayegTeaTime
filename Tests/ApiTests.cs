using System.Collections.Generic;
using System.Linq;
using HLE.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTimeCSharp.HttpRequests;
using OkayegTeaTimeCSharp.JsonData.JsonClasses.HttpRequests;
using OkayegTeaTimeCSharp.Twitch.API;
using OkayegTeaTimeCSharp.Twitch.Models;

namespace Tests;

[TestClass]
public class ApiTests
{
    private const string _testChannel = "strbhlfe";

    [TestMethod]
    public void GetSevenTvEmotesTest()
    {
        int emoteCount = 5;
        List<SevenTvEmote> emotes = HttpRequest.GetSevenTvEmotes(_testChannel, emoteCount).ToList();
        Assert.IsTrue(emotes.Count == emoteCount);
        emotes.ForEach(e =>
        {
            bool isMatch = e.Name.IsMatch(@"^\w+$");
            Assert.IsTrue(isMatch);
        });
    }

    [TestMethod]
    public void GetBTTVEmotesTest()
    {
        new TwitchAPI().Configure();
        int emoteCount = 5;
        List<BttvSharedEmote> emotes = HttpRequest.GetBttvEmotes(_testChannel, emoteCount).ToList();
        Assert.IsTrue(emotes.Count == emoteCount);
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
        List<Chatter> chatters = HttpRequest.GetChatters(_testChannel);
        chatters.ForEach(c =>
        {
            bool isMatch = c.Username.IsMatch(@"^\w+$");
            Assert.IsTrue(isMatch);
        });
    }

    [TestMethod]
    public void GetFFZEmotesTest()
    {
        int emoteCount = 5;
        List<FfzEmote> emotes = HttpRequest.GetFfzEmotes(_testChannel, emoteCount).ToList();
        Assert.IsTrue(emotes.Count == emoteCount);
        emotes.ForEach(e =>
        {
            bool isMatch = e.Name.IsMatch(@"^\w+$");
            Assert.IsTrue(isMatch);
        });
    }

    [TestMethod]
    public void GetMatchResultTest()
    {
        string result = HttpRequest.GetMathResult("2^8");
        Assert.AreEqual(result, "256");
    }

    [TestMethod]
    public void GetOnlineCompilerResultTest()
    {
        string result = HttpRequest.GetOnlineCompilerResult("\"test\".Out();");
        Assert.AreEqual(result, "test ");
    }
}
