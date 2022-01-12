using HLE.Time;
using OkayegTeaTime.Twitch.Commands.Enums;

namespace OkayegTeaTime.Twitch.Bot.Cooldowns;

public class Cooldown
{
    public int UserId { get; }

    public CommandType Type { get; }

    public long Time { get; }

    public Cooldown(int userId, CommandType type)
    {
        UserId = userId;
        Type = type;
        Time = TimeHelper.Now() + AppSettings.CommandList[type].Cooldown;
    }

    public override bool Equals(object? obj)
    {
        return obj is Cooldown c && c.UserId == UserId && c.Type == Type;
    }
}
