using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace OkayegTeaTime.Configuration;

[OptionsValidator]
public sealed partial class SettingsValidator : IValidateOptions<Settings>
{
    [RegexPattern]
    public const string TwitchUsernamePattern = @"(?i)^[a-z\d]\w{2,24}$";
}
