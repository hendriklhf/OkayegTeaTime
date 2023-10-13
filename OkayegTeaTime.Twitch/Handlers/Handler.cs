using System.Threading.Tasks;
using HLE.Twitch.Models;

namespace OkayegTeaTime.Twitch.Handlers;

public abstract class Handler(TwitchBot twitchBot)
{
    private protected readonly TwitchBot _twitchBot = twitchBot;

    public abstract ValueTask Handle(IChatMessage chatMessage);
}
