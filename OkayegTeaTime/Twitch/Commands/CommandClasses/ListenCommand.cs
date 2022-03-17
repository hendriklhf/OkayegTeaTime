using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class ListenCommand : Command
{
    public ListenCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, Prefix, @"\s\w+");
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Task.Run(async () =>
            {
                SpotifyUser? user = DbControl.SpotifyUsers[ChatMessage.Username];
                if (user is null)
                {
                    Response = $"{ChatMessage.Username}, you can't listen to other users, you have to register first";
                    return;
                }

                SpotifyUser? target = DbControl.SpotifyUsers[ChatMessage.LowerSplit[1]];
                if (target is null)
                {
                    Response = $"{ChatMessage.Username}, you can't listen to {ChatMessage.LowerSplit[1]}'s music, they have to register first";
                    return;
                }

                await user.ListenTo(target);
                Response = $"{ChatMessage.Username}, {user.Response}";
            }).Wait();
        }
    }
}
