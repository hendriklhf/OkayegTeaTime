using System;
using System.Diagnostics.Contracts;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch;

public abstract class CachedModel
{
    [JsonIgnore]
    private readonly DateTime _timeOfRequest = DateTime.UtcNow;

    [Pure]
    public bool IsValid(TimeSpan cacheTime) => _timeOfRequest + cacheTime > DateTime.UtcNow;
}
