using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Messages
{
    public static class TwitchLibHelper
    {
        public static string[] GetLowerSplit(this ChatMessage chatMessage)
        {
            return chatMessage.Message.SplitToLowerCase();
        }

        public static string GetMessage(this ChatMessage chatMessage)
        {
            return chatMessage.Message.MakeUsable();
        }

        public static string[] GetSplit(this ChatMessage chatMessage)
        {
            return chatMessage.Message.SplitNormal();
        }

        public static string[] GetLowerSplit(this WhisperMessage whisperMessage)
        {
            return whisperMessage.Message.SplitToLowerCase();
        }

        public static string GetMessage(this WhisperMessage whisperMessage)
        {
            return whisperMessage.Message.MakeUsable();
        }

        public static string[] GetSplit(this WhisperMessage whisperMessage)
        {
            return whisperMessage.Message.SplitNormal();
        }
    }
}
