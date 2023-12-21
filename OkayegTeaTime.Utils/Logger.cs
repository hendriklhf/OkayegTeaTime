using System;
using System.IO;

namespace OkayegTeaTime.Utils;

public static class Logger
{
    private const string LogsPath = "./Logs.log";

    public static void Log(string text) => LogToFile(text);

    private static string CreateLog(string input) => $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} | {input}{Environment.NewLine}";

    private static void LogToFile(string log) => File.AppendAllText(LogsPath, CreateLog(log));
}
