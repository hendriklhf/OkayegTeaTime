using System;
using System.Threading.Tasks;
using HLE.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

public interface IChatCommand<T> : IDisposable where T : IChatCommand<T>
{
    public ResponseBuilder Response { get; }

    public static abstract void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out T command);

    public ValueTask Handle();

    public new void Dispose()
    {
        Response.Dispose();
    }
}
