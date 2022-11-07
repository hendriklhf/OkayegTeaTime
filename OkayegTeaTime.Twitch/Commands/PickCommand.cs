using System.Text.RegularExpressions;
using HLE.Collections;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Pick)]
public sealed class PickCommand : Command
{
    public PickCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s\S+");
        if (!pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, no items provided";
            return;
        }

        Response = $"{ChatMessage.Username}, {ChatMessage.Split[1..].Random()!}";
    }
}
