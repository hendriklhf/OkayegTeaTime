using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Models.Formula1;

namespace OkayegTeaTime.Twitch.Json;

[JsonSerializable(typeof(Race[]))]
public sealed partial class Formula1JsonSerializerContext : JsonSerializerContext;
