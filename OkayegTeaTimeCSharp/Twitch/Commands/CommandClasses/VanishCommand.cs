using OkayegTeaTimeCSharp.Twitch.Bot;
using OkayegTeaTimeCSharp.Twitch.Messages.Interfaces;
using TwitchLib.Client.Extensions;

namespace OkayegTeaTimeCSharp.Twitch.Commands.CommandClasses;

public class VanishCommand : Command
{
    public VanishCommand(TwitchBot twitchBot, ITwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (!ChatMessage.IsModerator || !ChatMessage.IsBroadcaster || !ChatMessage.IsStaff)
        {
            TwitchBot.TwitchClient.TimeoutUser(ChatMessage.Channel.Name, ChatMessage.Username, TimeSpan.FromSeconds(1));
        }
    }
}
