using HLE.Strings;
using OkayegTeaTimeCSharp.Commands;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Messages
{
    public static class MessageHelper
    {
        public static bool IsAfkCommand(this ChatMessage chatMessage)
        {
            return CommandHelper.GetAfkCommandAliases().Any(alias => chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"(\s|$)")));
        }

        public static bool IsAnyCommand(this ChatMessage chatMessage)
        {
            return CommandHelper.GetAllAliases().Any(alias => chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"(\s|$)")));
        }

        public static bool IsCommand(this ChatMessage chatMessage)
        {
            return CommandHelper.GetCommandAliases().Any(alias => chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"(\s|$)")));
        }

        public static bool IsModOrBroadcaster(this ChatMessage chatMessage)
        {
            return chatMessage.IsModerator || chatMessage.IsBroadcaster;
        }

        public static bool IsNotLoggedChannel(this string channel)
        {
            return Config.NotLoggedChannels.Contains(channel);
        }

        public static bool IsSpecialUser(this string username)
        {
            return JsonHelper.BotData.UserLists.SpecialUsers.Contains(username);
        }

        public static byte[] MakeInsertable(this string input)
        {
            return input.Prepare().Encode();
        }

        public static string MakeQueryable(this string input)
        {
            return input.Prepare().RemoveSQLChars();
        }

        public static string MakeUsable(this string input)
        {
            return input.Prepare();
        }

        public static string Prepare(this string input)
        {
            return input.Remove(Resources.ChatterinoChar).Trim().ReplaceSpaces();
        }

        public static string[] SplitNormal(this string input)
        {
            return input.Prepare().Split();
        }

        public static string[] SplitToLowerCase(this string input)
        {
            return input.Prepare().ToLower().Split();
        }
    }
}