using HLE.Time;
using OkayegTeaTime.Twitch.Commands.Enums;

namespace OkayegTeaTime.Twitch.Bot.Cooldowns;

public class CooldownController
{
    public List<Cooldown> Cooldowns { get; } = new();

    public List<AfkCooldown> AfkCooldowns { get; } = new();

    public void AddCooldown(int userId, CommandType type)
    {
#if !DEBUG
        if (AppSettings.UserLists.Moderators.Contains(userId))
        {
            return;
        }
#endif

        Cooldown? cooldown = Cooldowns.FirstOrDefault(c => c.UserId == userId && c.Type == type);
        if (cooldown is not null)
        {
            Cooldowns.Remove(cooldown);
        }
        Cooldowns.Add(new(userId, type));
    }

    public void AddAfkCooldown(int userId)
    {
#if !DEBUG
        if (AppSettings.UserLists.Moderators.Contains(userId))
        {
            return;
        }
#endif

        AfkCooldown? cooldown = AfkCooldowns.FirstOrDefault(c => c.UserId == userId);
        if (cooldown is not null)
        {
            AfkCooldowns.Remove(cooldown);
        }
        AfkCooldowns.Add(new(userId));
    }

    public bool IsOnCooldown(int userId, CommandType type)
    {
        return Cooldowns.Any(c => c.UserId == userId && c.Type == type && c.Time > TimeHelper.Now());
    }

    public bool IsOnAfkCooldown(int userId)
    {
        return AfkCooldowns.Any(c => c.UserId == userId && c.Time > TimeHelper.Now());
    }
}
