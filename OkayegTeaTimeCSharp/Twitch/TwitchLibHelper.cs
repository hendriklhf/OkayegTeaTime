using OkayegTeaTimeCSharp.Twitch.Messages;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Twitch
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

        public static bool IsModOrBroadcaster(this ChatMessage chatMessage)
        {
            return chatMessage.IsModerator || chatMessage.IsBroadcaster;
        }
    }
}
