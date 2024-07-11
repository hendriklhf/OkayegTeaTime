using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Models;

namespace OkayegTeaTime.Twitch.Json;

[JsonSerializable(typeof(DotNetFiddleResult))]
public sealed partial class DotnetFiddleJsonSerializerContext : JsonSerializerContext;
