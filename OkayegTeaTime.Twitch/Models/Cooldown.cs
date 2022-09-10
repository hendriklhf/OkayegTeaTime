using HLE.Time;

#pragma warning disable CS0659

namespace OkayegTeaTime.Twitch.Models;

public class Cooldown
{
    public long UserId { get; }

    public CommandType Type { get; }

    public long Time { get; }

    public Cooldown(long userId, int cmdCooldown, CommandType type)
    {
        UserId = userId;
        Type = type;
        Time = TimeHelper.Now() + cmdCooldown;
    }

    public override bool Equals(object? obj)
    {
        return obj is Cooldown c && c.UserId == UserId && c.Type == Type;
    }
}