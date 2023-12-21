using System.Text.Json.Serialization;

namespace OkayegTeaTime.Settings;

[JsonSerializable(typeof(Settings))]
public sealed partial class SettingsJsonSerializerContext : JsonSerializerContext;
