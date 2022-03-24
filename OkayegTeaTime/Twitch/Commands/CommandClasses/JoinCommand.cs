using System.Text.RegularExpressions;
using HLE.Strings;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class JoinCommand : Command
{
    public JoinCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\s#?\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            if (!ChatMessage.IsBotModerator)
            {
                Response = $"{ChatMessage.Username}, {PredefinedMessages.NoBotModerator}";
                return;
            }

            string channel = ChatMessage.LowerSplit[1];
            string response = _twitchBot.JoinChannel(channel.Remove("#"));
            Response = $"{ChatMessage.Username}, {response}";
        }
    }
}
