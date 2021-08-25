using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Twitch;
using System;
using System.IO;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Logging
{
    public static class Logger
    {
        public static void Log(string text)
        {
            File.AppendAllText(Resources.LogsPath, CreateLog(text));
        }

        public static void Log(ChatMessage chatMessage)
        {
            File.AppendAllText(Resources.LogsPath, CreateLog($"#{chatMessage.Channel}>{chatMessage.Username}: {chatMessage.GetMessage()}"));
        }

        public static void Log(Exception ex)
        {
            File.AppendAllText(Resources.LogsPath, CreateLog($"{ex.GetType().Name}: {ex.Message}: {ex.StackTrace}"));
        }

        private static string CreateLog(string input)
        {
            return $"{DateTime.Now:dd:MM:yy HH:mm:ss} | {input}\n";
        }
    }
}
