using System;
using System.IO;
using OkayegTeaTimeCSharp.Messages.Interfaces;
using OkayegTeaTimeCSharp.Properties;

namespace OkayegTeaTimeCSharp.Logging
{
    public static class Logger
    {
        public static void Log(string text)
        {
            LogToFile(CreateLog(text));
        }

        public static void Log(ITwitchChatMessage chatMessage)
        {
            LogToFile(CreateLog($"#{chatMessage.Channel}>{chatMessage.Username}: {chatMessage.Message}"));
        }

        public static void Log(Exception ex)
        {
            LogToFile(CreateLog($"{ex.GetType().Name}: {ex.Message}: {ex.StackTrace}"));
        }

        private static string CreateLog(string input)
        {
            return $"{DateTime.Now:dd:MM:yy HH:mm:ss} | {input}\n";
        }

        private static void LogToFile(string log)
        {
            File.AppendAllText(Paths.Logs, log);
        }
    }
}
