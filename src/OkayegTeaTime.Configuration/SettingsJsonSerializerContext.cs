using System.Text.Json.Serialization;

namespace OkayegTeaTime.Configuration;

[JsonSerializable(typeof(Settings))]
public sealed partial class SettingsJsonSerializerContext : JsonSerializerContext;
