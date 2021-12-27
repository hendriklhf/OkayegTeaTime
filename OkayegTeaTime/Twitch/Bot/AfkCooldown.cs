using HLE.Time;

namespace OkayegTeaTime.Twitch.Bot;

public class AfkCooldown
{
    public string Username { get; }

    public long Time { get; }

    public AfkCooldown(string username)
    {
        Username = username;
        Time = TimeHelper.Now() + AppSettings.AfkCooldown;
    }
}
