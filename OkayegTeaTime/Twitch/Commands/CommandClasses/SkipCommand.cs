using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OkayegTeaTime.Database;
using OkayegTeaTime.Database.Models;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands.CommandClasses;

public class SkipCommand : Command
{
    public SkipCommand(TwitchBot twitchBot, TwitchChatMessage chatMessage, string alias)
        : base(twitchBot, chatMessage, alias)
    {
    }

    public override void Handle()
    {
        Regex pattern = PatternCreator.Create(Alias, Prefix);
        if (pattern.IsMatch(ChatMessage.Message))
        {
            Response = $"{ChatMessage.Username}, ";
            if (ChatMessage.IsModerator || ChatMessage.IsBroadcaster)
            {
                Task.Run(async () =>
                {
                    SpotifyUser? user = DbControl.SpotifyUsers[ChatMessage.Channel];
                    if (user is null)
                    {
                        Response += $"you can't skip songs of {ChatMessage.Channel.Antiping()}, they have to register first";
                        return;
                    }

                    await user.Skip();
                    Response += $"{user.Response}";
                }).Wait();
            }
            else
            {
                Response += "you have to be a mod or the broadcaster to skip the song";
            }
        }
    }
}
