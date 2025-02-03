using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace OkayegTeaTime.Twitch.Models.Formula1;

[InlineArray(5)]
[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types")]
[SuppressMessage("Major Code Smell", "S3898:Value types should implement \"IEquatable<T>\"")]
[SuppressMessage("Roslynator", "RCS1213:Remove unused member declaration")]
[SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed")]
[SuppressMessage("Style", "IDE0044:Add readonly modifier")]
[SuppressMessage("Roslynator", "RCS1169:Make field read-only")]
[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members")]
public struct FiveSessionsBuffer
{
    private Sessionn _session;
}
