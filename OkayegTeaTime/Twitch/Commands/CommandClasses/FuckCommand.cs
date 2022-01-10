using System.Text.RegularExpressions;
using HLE.Emojis;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class FuckCommand : Command
{
    public FuckCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, ChatMessage.Channel.Prefix, @"\s\w+(\s\S+)?");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{Emoji.PointRight} {Emoji.OkHand} {ChatMessage.Username} fucked {ChatMessage.Split[1]}";
            if (ChatMessage.Split.Length > 2)
            {
                Response += $" {ChatMessage.Split[2]}";
            }
            return;
        }
    }
}
