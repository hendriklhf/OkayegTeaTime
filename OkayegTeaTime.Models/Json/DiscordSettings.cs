namespace OkayegTeaTime.Models.Json;

public sealed class DiscordSettings
{
    public required string PermissionInt { get; init; }

    public required string ApplicationId { get; init; }

    public required string Token { get; init; }
}
