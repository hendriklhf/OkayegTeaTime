using System;
using System.Diagnostics;
using OkayegTeaTime.Files;
using OkayegTeaTime.Logging;

namespace OkayegTeaTime.Utils;

public static class ProcessUtils
{
    public static void ConsoleOut(string value, ConsoleColor fontColor = ConsoleColor.Gray, bool logging = false)
    {
        Console.ForegroundColor = fontColor;
        try
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} | {value}");
        }
        finally
        {
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        if (logging)
        {
            Logger.Log(value);
        }
    }

    public static void Restart()
    {
        ConsoleOut("[SYSTEM] RESTARTED", ConsoleColor.Red, true);
        Process.Start($"./{AppSettings.AssemblyName}", Environment.GetCommandLineArgs());
        Environment.Exit(0);
    }
}
