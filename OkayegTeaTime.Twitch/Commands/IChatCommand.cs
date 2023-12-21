using System;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;

namespace OkayegTeaTime.Twitch.Commands;

public interface IChatCommand<T> : IDisposable, IEquatable<T> where T : IChatCommand<T>
{
    PooledStringBuilder Response { get; }

    static abstract void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, out T command);

    ValueTask Handle();
}
