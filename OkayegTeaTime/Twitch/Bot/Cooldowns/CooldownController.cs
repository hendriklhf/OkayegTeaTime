using HLE.Time;
using OkayegTeaTime.Twitch.Commands.Enums;

namespace OkayegTeaTime.Twitch.Bot.Cooldowns;

public static class CooldownController
{
    public static List<Cooldown> Cooldowns { get; } = new();

    public static List<AfkCooldown> AfkCooldowns { get; } = new();

    public static void AddCooldown(int userId, CommandType type)
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

    public static void AddAfkCooldown(int userId)
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

    public static bool IsOnCooldown(int userId, CommandType type)
    {
        return Cooldowns.Any(c => c.UserId == userId && c.Type == type && c.Time > TimeHelper.Now());
    }

    public static bool IsOnAfkCooldown(int userId)
    {
        return AfkCooldowns.Any(c => c.UserId == userId && c.Time > TimeHelper.Now());
    }
}
