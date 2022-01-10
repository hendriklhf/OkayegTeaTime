using HLE.Time;

namespace OkayegTeaTime.Twitch.Bot.Cooldowns;

public class AfkCooldown
{
    public int UserId { get; }

    public long Time { get; }

    public AfkCooldown(int userId)
    {
        UserId = userId;
        Time = TimeHelper.Now() + AppSettings.AfkCooldown;
    }
}
