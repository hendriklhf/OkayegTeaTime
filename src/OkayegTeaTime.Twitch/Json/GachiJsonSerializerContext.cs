using System.Collections.Immutable;
using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Json;

[JsonSerializable(typeof(ImmutableArray<GachiSong>))]
public sealed partial class GachiJsonSerializerContext : JsonSerializerContext;
