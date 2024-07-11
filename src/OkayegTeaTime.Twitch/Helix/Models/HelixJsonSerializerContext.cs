using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.Helix.Models.Responses;

namespace OkayegTeaTime.Twitch.Helix.Models;

[JsonSerializable(typeof(AccessToken))]
[JsonSerializable(typeof(GetResponse<User>))]
[JsonSerializable(typeof(GetResponse<Emote>))]
[JsonSerializable(typeof(GetResponse<Stream>))]
[JsonSerializable(typeof(GetResponse<ChannelEmote>))]
public sealed partial class HelixJsonSerializerContext : JsonSerializerContext;
