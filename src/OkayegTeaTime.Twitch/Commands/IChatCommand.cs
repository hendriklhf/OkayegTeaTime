using System;
using System.Threading.Tasks;
using HLE.Text;
using HLE.Twitch.Tmi.Models;
using JetBrains.Annotations;

namespace OkayegTeaTime.Twitch.Commands;

public interface IChatCommand<T> : IDisposable, IEquatable<T> where T : IChatCommand<T>
{
    PooledStringBuilder Response { get; }

    static abstract void Create(TwitchBot twitchBot, ChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, [MustDisposeResource] out T command);

    ValueTask HandleAsync();
}
