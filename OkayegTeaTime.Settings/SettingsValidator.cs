using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace OkayegTeaTime.Settings;

public static class SettingsValidator
{
    [RegexPattern]
    public const string TwitchUsernamePattern = @"(?i)^[a-z\d]\w{2,24}$";

    public static void Validate(Settings settings)
    {
        ValidationContext context = new(settings);
        Validator.ValidateObject(settings, context, true);
    }
}
