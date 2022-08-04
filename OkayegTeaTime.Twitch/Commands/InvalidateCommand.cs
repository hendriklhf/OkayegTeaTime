using System.Linq;
using System.Text.RegularExpressions;
using OkayegTeaTime.Database;
using OkayegTeaTime.Twitch.Attributes;
using OkayegTeaTime.Twitch.Models;
using OkayegTeaTime.Utils;

namespace OkayegTeaTime.Twitch.Commands;

[HandledCommand(CommandType.Invalidate)]
public class InvalidateCommand : Command
{
    public InvalidateCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        if (!ChatMessage.IsBotModerator)
        {
            Response = $"{ChatMessage.Username}, {PredefinedMessages.NoBotModerator}";
            return;
        }

        Regex pattern = PatternCreator.Create(_alias, _prefix, @"\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            string[] invalidatedCaches = DbControl.Invalidate(ChatMessage.Split[1]).ToArray();
            if (!invalidatedCaches.Any())
            {
                Response = $"{ChatMessage.Username}, no matching caches could be found";
                return;
            }

            Response = $"{ChatMessage.Username}, the following caches have been invalidated: {string.Join(", ", invalidatedCaches)}";
            return;
        }

        DbControl.Invalidate();
        Response = $"{ChatMessage.Username}, all database caches have been invalidated";
    }
}
