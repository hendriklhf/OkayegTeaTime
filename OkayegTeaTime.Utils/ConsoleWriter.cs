using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using HLE.Strings;
using HLE.Twitch.Models;

namespace OkayegTeaTime.Utils;

[SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "it is disposable")]
public readonly struct ConsoleWriter : IDisposable, IAsyncDisposable, IEquatable<ConsoleWriter>
{
    private readonly PooledStringBuilder _builder;
    private readonly ConsoleColor _previousColor = Console.ForegroundColor;

    private const int DefaultCapacity = 64;

    public ConsoleWriter() : this(DefaultCapacity)
    {
    }

    public ConsoleWriter(int capacity)
    {
        _builder = new(capacity);
        _builder.Append(DateTime.Now, "HH:mm:ss");
        _builder.Append(" | ");
    }

    public void Dispose()
    {
        Console.Out.WriteLine(_builder.WrittenSpan);
        _builder.Dispose();
        Console.ForegroundColor = _previousColor;
    }

    public async ValueTask DisposeAsync()
    {
        await Console.Out.WriteLineAsync(_builder.WrittenMemory);
        _builder.Dispose();
        Console.ForegroundColor = _previousColor;
    }

    public void WriteConnected()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        _builder.Append("CONNECTED");
    }

    public void WriteDisconnected()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        _builder.Append("DISCONNECTED");
    }

    public void WriteJoinedChannel(string channel)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        _builder.Append("JOINED <#", channel);
        _builder.Append('>');
    }

    public void WriteChatMessage(IChatMessage chatMessage)
        => _builder.Append("<#", chatMessage.Channel, "> ", chatMessage.Username, ": ", chatMessage.Message);

    public bool Equals(ConsoleWriter other) => _builder == other._builder;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is ConsoleWriter other && Equals(other);

    public override int GetHashCode() => _builder.GetHashCode();

    public static bool operator ==(ConsoleWriter left, ConsoleWriter right) => left.Equals(right);

    public static bool operator !=(ConsoleWriter left, ConsoleWriter right) => !(left == right);
}
