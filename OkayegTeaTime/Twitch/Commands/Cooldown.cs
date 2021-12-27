using HLE.Time;
using OkayegTeaTime.Twitch.Commands.Enums;

namespace OkayegTeaTime.Twitch.Commands;

public class Cooldown
{
    public string Username { get; }

    public CommandType Type { get; }

    public long Time { get; private set; }

    public Cooldown(string username, CommandType type)
    {
        Username = username;
        Type = type;
        Time = TimeHelper.Now() + AppSettings.CommandList[type].Cooldown;
    }
}
