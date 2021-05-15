using OkayegTeaTimeCSharp.Messages;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class SetCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\sprefix\s\S{1,10}")))
            {
                if (chatMessage.IsModOrBroadcaster())
                {
                    twitchBot.SendSetPrefix(chatMessage, chatMessage.GetLowerSplit()[2][..(chatMessage.GetLowerSplit()[2].Length > 10 ? 10 : chatMessage.GetLowerSplit()[2].Length)]);
                }
                else
                {
                    twitchBot.Send(chatMessage.Channel, $"{chatMessage.Username}, only mods and the broadcaster can set the prefix");
                }
            }
        }
    }
}
