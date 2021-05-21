using OkayegTeaTimeCSharp.Commands;
using OkayegTeaTimeCSharp.Commands.CommandEnums;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Utils;
using System.Linq;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Messages
{
    public static class MessageHelper
    {
        public static bool IsAfkCommand(string message)
        {
            return CommandHelper.GetAfkCommandAliases().Any(alias => message.IsMatch(PatternCreator.Create(alias, PrefixType.Active)) || message.IsMatch(PatternCreator.Create(alias, PrefixType.None)));
        }

        public static bool IsAnyCommand(string message)
        {
            return CommandHelper.GetAllAliases().Any(alias => message.IsMatch(PatternCreator.Create(alias, PrefixType.Active)) || message.IsMatch(PatternCreator.Create(alias, PrefixType.None)));
        }

        public static bool IsCommand(string message)
        {
            return CommandHelper.GetCommandAliases().Any(alias => message.IsMatch(PatternCreator.Create(alias, PrefixType.Active)) || message.IsMatch(PatternCreator.Create(alias, PrefixType.None)));
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
            return input.ReplaceChatterinoChar().Trim().ReplaceSpaces().EscapeChars().Encode();
        }

        public static string MakeQueryable(this string input)
        {
            return input.ReplaceChatterinoChar().Trim().ReplaceSpaces().EscapeChars();
        }

        public static string MakeUsable(this string input)
        {
            return input.ReplaceChatterinoChar().Trim().ReplaceSpaces();
        }

        public static string[] Split(this string input)
        {
            return input.ReplaceChatterinoChar().Trim().ReplaceSpaces().Split(" ");
        }

        public static string[] SplitToLowerCase(this string input)
        {
            return input.ReplaceChatterinoChar().Trim().ReplaceSpaces().ToLower().Split(" ");
        }
    }
}