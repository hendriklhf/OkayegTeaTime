using HLE.Time;
using OkayegTeaTime.Twitch.Commands.Enums;

namespace OkayegTeaTime.Twitch.Bot.Cooldowns;

public class Cooldown
{
    public int UserId { get; }

    public CommandType Type { get; }

    public long Time { get; private set; }

    public Cooldown(int userId, CommandType type)
    {
        UserId = userId;
        Type = type;
        Time = TimeHelper.Now() + AppSettings.CommandList[type].Cooldown;
    }
}
