using System.IO;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Logging;

public static class Logger
{
    private const string _logsPath = "./Resources/Logs.log";

    public static void Log(string text)
    {
        LogToFile(text);
    }

    public static void Log(TwitchChatMessage chatMessage)
    {
        LogToFile($"#{chatMessage.Channel}>{chatMessage.Username}: {chatMessage.Message}");
    }

    public static void Log(Exception ex)
    {
        string log = $"{ex.GetType().Name}: {ex.Message}: {ex.StackTrace}";
        ConsoleOut(log, ConsoleColor.Magenta);
        DbController.LogException(ex);
    }

    private static string CreateLog(string input)
    {
        return $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} | {input}{Environment.NewLine}";
    }

    private static void LogToFile(string log)
    {
        File.AppendAllText(_logsPath, CreateLog(log));
    }
}
