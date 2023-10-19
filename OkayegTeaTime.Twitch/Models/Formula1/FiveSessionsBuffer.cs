using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace OkayegTeaTime.Twitch.Models.Formula1;

[InlineArray(5)]
public struct FiveSessionsBuffer : IEquatable<FiveSessionsBuffer>
{
    #region private Session _session;

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    [SuppressMessage("Style", "IDE0044:Add readonly modifier")]
    private Session _session;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

    #endregion

    public readonly bool Equals(FiveSessionsBuffer other) =>
        this[0] == other[0] && this[1] == other[1] && this[2] == other[2] &&
        this[3] == other[3] && this[4] == other[4];

    // ReSharper disable once ArrangeModifiersOrder
    public override readonly bool Equals(object? obj) => obj is FiveSessionsBuffer other && Equals(other);

    // ReSharper disable once ArrangeModifiersOrder
    public override readonly int GetHashCode() => _session.GetHashCode();

    public static bool operator ==(FiveSessionsBuffer left, FiveSessionsBuffer right) => left.Equals(right);

    public static bool operator !=(FiveSessionsBuffer left, FiveSessionsBuffer right) => !left.Equals(right);
}
