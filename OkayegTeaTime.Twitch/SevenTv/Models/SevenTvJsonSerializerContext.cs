using System.Text.Json.Serialization;
using OkayegTeaTime.Twitch.SevenTv.Models.Responses;

namespace OkayegTeaTime.Twitch.SevenTv.Models;

[JsonSerializable(typeof(GetGlobalEmotesResponse))]
[JsonSerializable(typeof(GetChannelEmotesResponse))]
public sealed partial class SevenTvJsonSerializerContext : JsonSerializerContext;
