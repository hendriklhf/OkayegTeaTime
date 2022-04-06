using HLE.Time;
using OkayegTeaTime.Files.Jsons.CommandData;
using OkayegTeaTime.Twitch.Bot;
using OkayegTeaTime.Twitch.Bot.Cooldown;
using OkayegTeaTime.Twitch.Commands.Enums;

namespace OkayegTeaTime.Twitch.Controller;

public class CooldownController
{
    public List<Cooldown> Cooldowns { get; } = new();

    public List<AfkCooldown> AfkCooldowns { get; } = new();

    private readonly TwitchBot _twitchBot;

    public CooldownController(TwitchBot twitchBot)
    {
        _twitchBot = twitchBot;
    }

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

        Command cmd = _twitchBot.CommandController[type];
        Cooldowns.Add(new(userId, cmd, type));
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
