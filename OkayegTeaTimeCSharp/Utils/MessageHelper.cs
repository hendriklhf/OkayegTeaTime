using OkayegTeaTimeCSharp.Twitch.Models;

namespace OkayegTeaTimeCSharp.Utils;

public static class MessageHelper
{
    public static bool IsMessageTooLong(string message, string channel)
    {
        return IsMessageTooLong(message, new Channel(channel));
    }

    public static bool IsMessageTooLong(string message, Channel channel)
    {
        return $"{channel.Emote} {message} {Settings.ChatterinoChar}".Length > Config.MaxMessageLength;
    }
}
