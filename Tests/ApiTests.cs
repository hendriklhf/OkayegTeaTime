using HLE.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkayegTeaTimeCSharp.HttpRequests;
using OkayegTeaTimeCSharp.HttpRequests.Models;
using OkayegTeaTimeCSharp.Twitch.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class ApiTests
    {
        public const string TestChannel = "strbhlfe";

        [TestMethod]
        public void Get7TVEmotesTest()
        {
            int emoteCount = 5;
            List<Emote> emotes = HttpRequest.Get7TVEmotes(TestChannel, emoteCount);
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
            List<Emote> emotes = HttpRequest.GetBTTVEmotes(TestChannel, emoteCount);
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
            int chatterCount = HttpRequest.GetChatterCount(TestChannel);
            Assert.IsNotNull(chatterCount);
        }

        [TestMethod]
        public void GetChattersTest()
        {
            List<Chatter> chatters = HttpRequest.GetChatters(TestChannel);
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
            List<Emote> emotes = HttpRequest.GetFFZEmotes(TestChannel, emoteCount);
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
