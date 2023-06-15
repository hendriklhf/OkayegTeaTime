using System;
using System.Diagnostics.Contracts;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch;

public abstract class CachedModel
{
    [JsonIgnore]
    internal readonly DateTime _timeOfRequest = DateTime.UtcNow;

    [Pure]
    internal bool IsValid(TimeSpan cacheTime)
    {
        return _timeOfRequest + cacheTime > DateTime.UtcNow;
    }
}
