using System;

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
}
