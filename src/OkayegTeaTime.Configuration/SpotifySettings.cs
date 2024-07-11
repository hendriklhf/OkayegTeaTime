using System.ComponentModel.DataAnnotations;

namespace OkayegTeaTime.Configuration;

public sealed class SpotifySettings
{
    [RegularExpression("^[a-z0-9]{32}$")]
    public required string ClientId { get; init; }

    [RegularExpression("^[a-z0-9]{32}$")]
    public required string ClientSecret { get; init; }
}
