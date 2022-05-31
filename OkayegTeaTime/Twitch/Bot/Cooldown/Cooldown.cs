using HLE.Time;
using OkayegTeaTime.Files.Jsons.CommandData;
using OkayegTeaTime.Twitch.Commands.Enums;

namespace OkayegTeaTime.Twitch.Bot.Cooldown;

public class Cooldown
{
    public long UserId { get; }

    public CommandType Type { get; }

    public long Time { get; }

    public Cooldown(long userId, Command cmd, CommandType type)
    {
        UserId = userId;
        Type = type;
        Time = TimeHelper.Now() + cmd.Cooldown;
    }

    public override bool Equals(object? obj)
    {
        return obj is Cooldown c && c.UserId == UserId && c.Type == Type;
    }
}
