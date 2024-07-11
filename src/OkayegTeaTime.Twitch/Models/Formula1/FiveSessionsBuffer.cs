using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace OkayegTeaTime.Twitch.Models.Formula1;

[InlineArray(5)]
public struct FiveSessionsBuffer : IEquatable<FiveSessionsBuffer>
{
    #region private Session _session;

#pragma warning disable RCS1169 // make readonly
    [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "members of an InlineArray can't be readonly")]
    private Session _session;
#pragma warning restore RCS1169

    #endregion private Session _session;

    [SuppressMessage("Style", "IDE0251:Make member \'readonly\'", Justification = "readonly results in a copy of this on every access")]
    public bool Equals(FiveSessionsBuffer other) =>
        this[0] == other[0] && this[1] == other[1] && this[2] == other[2] &&
        this[3] == other[3] && this[4] == other[4];

    // ReSharper disable once ArrangeModifiersOrder
    public override bool Equals(object? obj) => obj is FiveSessionsBuffer other && Equals(other);

    // ReSharper disable once ArrangeModifiersOrder
    public override readonly int GetHashCode() => _session.GetHashCode();

    public static bool operator ==(FiveSessionsBuffer left, FiveSessionsBuffer right) => left.Equals(right);

    public static bool operator !=(FiveSessionsBuffer left, FiveSessionsBuffer right) => !left.Equals(right);
}
