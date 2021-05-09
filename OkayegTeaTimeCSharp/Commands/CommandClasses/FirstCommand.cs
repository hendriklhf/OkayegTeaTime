using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public static class FirstCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\s\w+\s#?\w+")))
            {
                twitchBot.SendFirstUserChannel(chatMessage, chatMessage.GetLowerSplit()[1], chatMessage.GetLowerSplit()[2]);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\s#\w+")))
            {
                twitchBot.SendFirstChannel(chatMessage, chatMessage.GetLowerSplit()[1]);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"\s\w+$")))
            {
                twitchBot.SendFirstUser(chatMessage, chatMessage.GetLowerSplit()[1]);
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.CreateBoth(alias, @"$")))
            {
                twitchBot.SendFirst(chatMessage);
            }
        }
    }
}
