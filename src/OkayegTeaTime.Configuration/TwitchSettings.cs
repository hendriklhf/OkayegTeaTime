using System.ComponentModel.DataAnnotations;

namespace OkayegTeaTime.Configuration;

public sealed class TwitchSettings
{
    [RegularExpression(SettingsValidator.TwitchUsernamePattern)]
    public required string Username { get; init; }

    [RegularExpression("^oauth:[a-z0-9]{30}$")]
    public required string OAuthToken { get; init; }

    [RegularExpression("^[a-z0-9]{30}$")]
    public required string ApiClientId { get; init; }

    [RegularExpression("^[a-z0-9]{30}$")]
    public required string ApiClientSecret { get; init; }
}
