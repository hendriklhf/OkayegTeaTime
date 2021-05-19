using OkayegTeaTimeCSharp.Messages;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch
{
    public static class TwitchLibHelper
    {
        public static string GetMessage(this ChatMessage chatMessage)
        {
            return chatMessage.Message.MakeUsable();
        }

        public static string[] GetSplit(this ChatMessage chatMessage)
        {
            return chatMessage.Message.Split();
        }

        public static string[] GetLowerSplit(this ChatMessage chatMessage)
        {
            return chatMessage.Message.SplitToLowerCase();
        }
    }
}