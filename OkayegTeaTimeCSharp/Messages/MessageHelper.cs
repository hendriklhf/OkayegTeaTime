using OkayegTeaTimeCSharp.Commands;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Utils;
using Sterbehilfe.Strings;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Messages
{
    public static class MessageHelper
    {
        public static bool IsAfkCommand(ChatMessage chatMessage)
        {
            return CommandHelper.GetAfkCommandAliases().Any(alias => chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"(\s|$)")));
        }

        public static bool IsAnyCommand(ChatMessage chatMessage)
        {
            return CommandHelper.GetAllAliases().Any(alias => chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"(\s|$)")));
        }

        public static bool IsCommand(ChatMessage chatMessage)
        {
            return CommandHelper.GetCommandAliases().Any(alias => chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"(\s|$)")));
        }

        public static bool IsModOrBroadcaster(this ChatMessage chatMessage)
        {
            return chatMessage.IsModerator || chatMessage.IsBroadcaster;
        }

        public static bool IsNotLoggedChannel(string channel)
        {
            return Config.GetNotLoggedChannels().Contains(channel);
        }

        public static bool IsSpecialUser(string username)
        {
            return JsonHelper.BotData.UserLists.SpecialUsers.Contains(username);
        }

        public static byte[] MakeInsertable(this string input)
        {
            return input.ReplaceChatterinoChar().Trim().ReplaceSpaces().Encode();
        }

        public static string MakeQueryable(this string input)
        {
            return input.ReplaceChatterinoChar().Trim().ReplaceSpaces().EscapeChars();
        }

        public static string MakeUsable(this string input)
        {
            return input.ReplaceChatterinoChar().Trim().ReplaceSpaces();
        }

        public static string[] SplitNormal(this string input)
        {
            return input.ReplaceChatterinoChar().Trim().ReplaceSpaces().Split();
        }

        public static string[] SplitToLowerCase(this string input)
        {
            return input.ReplaceChatterinoChar().Trim().ReplaceSpaces().ToLower().Split();
        }
    }
}