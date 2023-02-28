using System;
using OkayegTeaTime.Settings;

#pragma warning disable CS0659

namespace OkayegTeaTime.Twitch.Models;

public sealed class AfkCooldown
{
    public long UserId { get; }

    public DateTimeOffset Until { get; }

    public AfkCooldown(long userId)
    {
        UserId = userId;
        Until = DateTimeOffset.UtcNow.AddMilliseconds(AppSettings.AfkCooldown);
    }

    public override bool Equals(object? obj)
    {
        return obj is AfkCooldown c && c.UserId == UserId;
    }
}
