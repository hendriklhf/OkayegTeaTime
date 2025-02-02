using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;

namespace OkayegTeaTime.Configuration;

public static class GlobalSettings
{
    public static string AssemblyName { get; } = Assembly.GetEntryAssembly()!.GetName().Name!;

    public static Settings Settings { get; private set; } = null!;

    public const int AfkCooldown = 10000;
    public const string DefaultEmote = "Okayeg";
    public const int MaxEmoteInFrontLength = 20;
    public const int MaxMessageLength = 500;
    public const int MaxPrefixLength = 10;
    public const int MaxReminders = 10;
    public const int DelayBetweenSentMessages = 1300;
    public const string Suffix = "eg";
    public const string SettingsFileName = "Settings.json";
    public const string ChatterinoChar = "\uDB40\uDC00";
    public const string HleNugetVersionId = "111893";

    public static void Initialize()
    {
        Settings settings = SettingsReader.Read(SettingsFileName);
        SettingsValidator validator = new();
        ValidateOptionsResult validationResult = validator.Validate(null, settings);
        if (validationResult.Failed)
        {
            ThrowValidationFailed(validationResult);
        }

        Settings = settings;

        return;

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ThrowValidationFailed(ValidateOptionsResult validationResult)
            => throw new OptionsValidationException(nameof(Settings), typeof(Settings), validationResult.Failures);
    }
}
