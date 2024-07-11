using System;
using System.Collections.Concurrent;
using HLE.Collections;
using OkayegTeaTime.Configuration;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Controller;

public sealed class CooldownController
{
    private readonly ConcurrentDictionary<CooldownHash, DateTime> _cooldowns = new();
    private readonly ConcurrentDictionary<CooldownHash, DateTime> _afkCooldowns = new();

    public void AddCooldown(long userId, CommandType type)
    {
#if DEBUG
        if (GlobalSettings.Settings.Users.Moderators.Contains(userId))
        {
            return;
        }
#endif

        CooldownHash cooldownHash = new(userId, type);
#pragma warning disable S6354
        DateTime cooldownUntil = DateTime.UtcNow + CommandController.GetCommand(type).Cooldown;
#pragma warning restore S6354
        _cooldowns.AddOrSet(cooldownHash, cooldownUntil);
    }

    public void AddAfkCooldown(long userId)
    {
#if DEBUG
        if (GlobalSettings.Settings.Users.Moderators.Contains(userId))
        {
            return;
        }
#endif

        CooldownHash cooldownHash = new(userId);
        TimeSpan cooldownTime = TimeSpan.FromMilliseconds(GlobalSettings.AfkCooldown);
#pragma warning disable S6354
        DateTime cooldownUntil = DateTime.UtcNow + cooldownTime;
#pragma warning restore S6354
        _afkCooldowns.AddOrSet(cooldownHash, cooldownUntil);
    }

    public bool IsOnCooldown(long userId, CommandType type)
    {
        CooldownHash cooldownHash = new(userId, type);
#pragma warning disable S6354
        return _cooldowns.TryGetValue(cooldownHash, out DateTime cooldownUntil) && cooldownUntil > DateTime.UtcNow;
#pragma warning restore S6354
    }

    public bool IsOnAfkCooldown(long userId)
    {
        CooldownHash cooldownHash = new(userId);
#pragma warning disable S6354
        return _afkCooldowns.TryGetValue(cooldownHash, out DateTime cooldownUntil) && cooldownUntil > DateTime.UtcNow;
#pragma warning restore S6354
    }
}
