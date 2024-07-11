using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch;

public abstract class CachedModel
{
    [JsonIgnore]
    [SuppressMessage("Major Code Smell", "S6354:Use a testable date/time provider")]
    private readonly DateTime _timeOfRequest = DateTime.UtcNow;

    [Pure]
    [SuppressMessage("Major Code Smell", "S6354:Use a testable date/time provider")]
    public bool IsValid(TimeSpan cacheTime) => _timeOfRequest + cacheTime > DateTime.UtcNow;
}
