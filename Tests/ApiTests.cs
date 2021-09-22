using System.Collections.Generic;
using HLE.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTimeCSharp.HttpRequests;
using OkayegTeaTimeCSharp.Models;
using OkayegTeaTimeCSharp.Twitch.API;

namespace Tests
{
    [TestClass]
    public class ApiTests
    {
        private const string _testChannel = "strbhlfe";

        [TestMethod]
        public void Get7TVEmotesTest()
        {
            int emoteCount = 5;
            List<Emote> emotes = HttpRequest.Get7TVEmotes(_testChannel, emoteCount);
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
            List<Emote> emotes = HttpRequest.GetBTTVEmotes(_testChannel, emoteCount);
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
            List<Emote> emotes = HttpRequest.GetFFZEmotes(_testChannel, emoteCount);
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
            Assert.AreEqual(result, "test\r\n");
        }
    }
}
