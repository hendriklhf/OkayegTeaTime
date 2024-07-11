using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Ffz.Models.Responses;

namespace OkayegTeaTime.Twitch.Ffz.Models;

[JsonSerializable(typeof(GetRoomResponse))]
[JsonSerializable(typeof(GetGlobalEmotesResponse))]
public sealed partial class FfzJsonSerializerContext : JsonSerializerContext;
