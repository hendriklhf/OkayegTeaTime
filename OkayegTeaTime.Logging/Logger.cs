using System;
using System.IO;

namespace OkayegTeaTime.Logging;

public static class Logger
{
    private const string _logsPath = "./Logs.log";

    public static void Log(string text)
    {
        LogToFile(text);
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
