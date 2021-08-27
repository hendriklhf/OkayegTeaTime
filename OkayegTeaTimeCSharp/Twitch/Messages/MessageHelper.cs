using HLE.Strings;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Commands;
using OkayegTeaTimeCSharp.Utils;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch.Messages
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

        public static bool IsNotLoggedChannel(this string channel)
        {
            return Config.NotLoggedChannels.Contains(channel);
        }

        public static bool IsSpecialUser(this string username)
        {
            return new JsonController().BotData.UserLists.SpecialUsers.Contains(username);
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
