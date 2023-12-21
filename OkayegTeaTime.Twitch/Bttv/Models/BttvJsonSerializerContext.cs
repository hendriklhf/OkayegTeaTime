using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Bttv.Models.Responses;

namespace OkayegTeaTime.Twitch.Bttv.Models;

[JsonSerializable(typeof(GetUserResponse))]
public sealed partial class BttvJsonSerializerContext : JsonSerializerContext;
