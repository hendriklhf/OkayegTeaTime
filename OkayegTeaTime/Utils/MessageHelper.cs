using OkayegTeaTime.Database;

namespace OkayegTeaTime.Utils;

public static class MessageHelper
{
    public static bool IsMessageTooLong(string message, string channel)
    {
        string emote = DbControl.Channels[channel]?.Emote ?? AppSettings.DefaultEmote;
        return $"{emote} {message} {AppSettings.ChatterinoChar}".Length > AppSettings.MaxMessageLength;
    }
}
