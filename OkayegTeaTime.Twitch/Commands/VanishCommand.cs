using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Vanish)]
public class VanishCommand : Command
{
    public VanishCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias) : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (!ChatMessage.IsModerator && !ChatMessage.IsBroadcaster && !ChatMessage.IsStaff)
        {
            _twitchBot.SendText(ChatMessage.Channel, $"/timeout {ChatMessage.Username} 1");
        }
    }
}
