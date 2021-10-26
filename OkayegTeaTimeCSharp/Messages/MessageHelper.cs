using HLE.Strings;
using OkayegTeaTimeCSharp.Commands;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Utils;

namespace OkayegTeaTimeCSharp.Messages;

public static class MessageHelper
{
    public static bool IsAfkCommand(this ITwitchChatMessage chatMessage)
    {
        return CommandHelper.GetAfkCommandAliases().Any(alias => chatMessage.Message.IsMatch(PatternCreator.Create(alias, chatMessage.Channel.Prefix, @"(\s|$)")));
    }

    public static bool IsAnyCommand(this ITwitchChatMessage chatMessage)
    {
        return CommandHelper.GetAllAliases().Any(alias => chatMessage.Message.IsMatch(PatternCreator.Create(alias, chatMessage.Channel.Prefix, @"(\s|$)")));
    }

    public static bool IsCommand(this ITwitchChatMessage chatMessage)
    {
        return CommandHelper.GetCommandAliases().Any(alias => chatMessage.Message.IsMatch(PatternCreator.Create(alias, chatMessage.Channel.Prefix, @"(\s|$)")));
    }

    public static bool IsNotLoggedChannel(this string channel)
    {
        return TwitchConfig.NotLoggedChannels.Contains(channel);
    }

    public static bool IsSpecialUser(this string username)
    {
        return TwitchConfig.SpecialUsers.Contains(username);
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

    private static string Prepare(this string input)
    {
        return input.Remove(Settings.ChatterinoChar).TrimAll();
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
