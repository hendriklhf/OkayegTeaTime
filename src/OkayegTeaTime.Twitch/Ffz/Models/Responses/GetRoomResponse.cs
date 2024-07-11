using System;
using System.Text.Json.Serialization;

namespace OkayegTeaTime.Twitch.Ffz.Models.Responses;

public readonly struct GetRoomResponse : IEquatable<GetRoomResponse>
{
    [JsonPropertyName("room")]
    public required Room Room { get; init; } = Room.Empty;

    public GetRoomResponse()
    {
    }

    public bool Equals(GetRoomResponse other) => Room.Equals(other.Room);

    public override bool Equals(object? obj) => obj is GetRoomResponse other && Equals(other);

    public override int GetHashCode() => Room.GetHashCode();

    public static bool operator ==(GetRoomResponse left, GetRoomResponse right) => left.Equals(right);

    public static bool operator !=(GetRoomResponse left, GetRoomResponse right) => !left.Equals(right);
}
