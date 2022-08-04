using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Files.Models;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Rafk)]
public class RafkCommand : Command
{
    public RafkCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        User? user = DbControl.Users.GetUser(ChatMessage.UserId, ChatMessage.Username);
        if (user is null)
        {
            Response = $"{ChatMessage.Username}, can't resume your afk status, because you never went afk before";
            return;
        }

        user.IsAfk = true;
        AfkCommand cmd = _twitchBot.CommandController[user.AfkType];
        Response = new AfkMessage(user, cmd).Resuming;
    }
}
