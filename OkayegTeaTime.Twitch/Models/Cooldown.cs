using System;

#pragma warning disable CS0659

namespace OkayegTeaTime.Twitch.Models;

public sealed class Cooldown
{
    public long UserId { get; }

    public CommandType Type { get; }

    public DateTimeOffset Until { get; }

    public Cooldown(long userId, int cmdCooldown, CommandType type)
    {
        UserId = userId;
        Type = type;
        Until = DateTimeOffset.UtcNow.AddMilliseconds(cmdCooldown);
    }

    public override bool Equals(object? obj)
    {
        return obj is Cooldown c && c.UserId == UserId && c.Type == Type;
    }
}
