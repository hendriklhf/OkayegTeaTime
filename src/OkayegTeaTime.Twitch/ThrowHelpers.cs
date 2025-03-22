using System;
using System.Diagnostics.CodeAnalysis;

namespace OkayegTeaTime.Twitch;

internal static class ThrowHelpers
{
    [DoesNotReturn]
    public static void ThrowObjectDisposedException<T>() where T : allows ref struct
        => throw new ObjectDisposedException(typeof(T).FullName);
}
