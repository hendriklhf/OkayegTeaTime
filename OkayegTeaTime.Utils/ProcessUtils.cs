using System;
using System.Diagnostics;
using OkayegTeaTime.Settings;

namespace OkayegTeaTime.Utils;

public static class ProcessUtils
{
    public static void ConsoleOut(string value, ConsoleColor fontColor = ConsoleColor.Gray, bool logging = false)
    {
        ConsoleColor previousForegroundColor = Console.ForegroundColor;
        Console.ForegroundColor = fontColor;
        try
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} | {value}");
        }
        finally
        {
            Console.ForegroundColor = previousForegroundColor;
        }

        if (logging)
        {
            Logger.Log(value);
        }
    }

    public static void Restart()
    {
        ConsoleOut("[SYSTEM] RESTARTED", ConsoleColor.Red, true);
        Process.Start($"./{GlobalSettings.AssemblyName}", Environment.GetCommandLineArgs());
        Environment.Exit(0);
    }
}
