using System;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;
using JetBrains.Annotations;

namespace OkayegTeaTime.Twitch.Commands;

public interface IChatCommand<T> : IDisposable, IEquatable<T> where T : IChatCommand<T>
{
    PooledStringBuilder Response { get; }

    static abstract void Create(TwitchBot twitchBot, IChatMessage chatMessage, ReadOnlyMemory<char> prefix, ReadOnlyMemory<char> alias, [MustDisposeResource] out T command);

    // ReSharper disable once InconsistentNaming
    ValueTask HandleAsync();
}
