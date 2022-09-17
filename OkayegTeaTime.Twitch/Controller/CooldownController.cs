using System.Collections.Generic;
using System.Linq;
using HLE.Time;
#if DEBUG
using OkayegTeaTime.Files;
#endif
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class CooldownController
{
    private readonly List<Cooldown> _cooldowns = new();
    private readonly List<AfkCooldown> _afkCooldowns = new();

    private readonly CommandController _commandController;

    public CooldownController(CommandController commandController)
    {
        _commandController = commandController;
    }

    public void AddCooldown(long userId, CommandType type)
    {
#if DEBUG
        if (AppSettings.UserLists.Moderators.Contains(userId))
        {
            return;
        }
#endif

        Cooldown? cooldown = _cooldowns.FirstOrDefault(c => c.UserId == userId && c.Type == type);
        if (cooldown is not null)
        {
            _cooldowns.Remove(cooldown);
        }

        int cmdCooldown = _commandController[type].Cooldown;
        _cooldowns.Add(new(userId, cmdCooldown, type));
    }

    public void AddAfkCooldown(long userId)
    {
#if DEBUG
        if (AppSettings.UserLists.Moderators.Contains(userId))
        {
            return;
        }
#endif

        AfkCooldown? cooldown = _afkCooldowns.FirstOrDefault(c => c.UserId == userId);
        if (cooldown is not null)
        {
            _afkCooldowns.Remove(cooldown);
        }

        _afkCooldowns.Add(new(userId));
    }

    public bool IsOnCooldown(long userId, CommandType type)
    {
        return _cooldowns.Any(c => c.UserId == userId && c.Type == type && c.Time > TimeHelper.Now());
    }

    public bool IsOnAfkCooldown(long userId)
    {
        return _afkCooldowns.Any(c => c.UserId == userId && c.Time > TimeHelper.Now());
    }
}
