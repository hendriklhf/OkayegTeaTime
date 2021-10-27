using System.IO;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;
using Path = OkayegTeaTimeCSharp.Properties.Path;

namespace OkayegTeaTimeCSharp.Logging;

public static class Logger
{
    public static void Log(string text)
    {
        LogToFile(text);
    }

    public static void Log(ITwitchChatMessage chatMessage)
    {
        LogToFile($"#{chatMessage.Channel}>{chatMessage.Username}: {chatMessage.Message}");
    }

    public static void Log(Exception ex)
    {
        LogToFile($"{ex.GetType().Name}: {ex.Message}: {ex.StackTrace}");
    }

    private static string CreateLog(string input)
    {
        return $"{DateTime.Now:dd:MM:yy HH:mm:ss} | {input}{Environment.NewLine}";
    }

    private static void LogToFile(string log)
    {
        File.AppendAllText(Path.Logs, CreateLog(log));
    }
}
