using HLE.Emojis;
using HLE.Strings;
using OkayegTeaTimeCSharp.Properties;
using OkayegTeaTimeCSharp.Twitch;
using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Utils;
using TwitchLib.Client.Models;

namespace OkayegTeaTimeCSharp.Commands.CommandClasses
{
    public class HelpCommand
    {
        public static void Handle(TwitchBot twitchBot, ChatMessage chatMessage, string alias)
        {
            if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel), @"\s\w+")))
            {
                twitchBot.Send(chatMessage.Channel, $"{Emoji.PointRight} {chatMessage.GetLowerSplit()[1]}, here you can find a list of commands and the repository: {Resources.GitHubRepoLink}");
            }
            else if (chatMessage.GetMessage().IsMatch(PatternCreator.Create(alias, PrefixHelper.GetPrefix(chatMessage.Channel))))
            {
                twitchBot.Send(chatMessage.Channel, $"{Emoji.PointRight} {chatMessage.Username}, here you can find a list of commands and the repository: {Resources.GitHubRepoLink}");
            }
        }
    }
}