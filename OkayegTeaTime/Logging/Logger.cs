using System.IO;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Logging;

public static class Logger
{
    public static void Log(string text)
    {
        LogToFile(Path.ExceptionLog, text);
    }

    public static void Log(TwitchChatMessage chatMessage)
    {
        LogToFile(Path.Logs, $"#{chatMessage.Channel}>{chatMessage.Username}: {chatMessage.Message}");
    }

    public static void Log(Exception ex)
    {
        string log = $"{ex.GetType().Name}: {ex.Message}: {ex.StackTrace}";
        ConsoleOut(log, ConsoleColor.Magenta);
        LogToFile(Path.ExceptionLog, log);
    }

    private static string CreateLog(string input)
    {
        return $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} | {input}{Environment.NewLine}";
    }

    private static void LogToFile(string path, string log)
    {
        File.AppendAllText(path, CreateLog(log));
    }
}
