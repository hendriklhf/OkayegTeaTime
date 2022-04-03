using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;
using TwitchLib.Client.Extensions;

namespace OkayegTeaTime.Twitch.Commands;

public class VanishCommand : Command
{
    public VanishCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (!ChatMessage.IsModerator && !ChatMessage.IsBroadcaster && !ChatMessage.IsStaff)
        {
            _twitchBot.TwitchClient.TimeoutUser(ChatMessage.Channel, ChatMessage.Username, TimeSpan.FromSeconds(1));
        }
    }
}
