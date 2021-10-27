using OkayegTeaTimeCSharp.Database;
using OkayegTeaTimeCSharp.JsonData;
using OkayegTeaTimeCSharp.Properties;

namespace OkayegTeaTimeCSharp;

public static class Config
{
    public const short AfkCooldown = 10000;
    public const string DefaultEmote = "Okayeg";
    public const byte MaxEmoteInFrontLength = 20;
    public const short MaxMessageLength = 500;
    public const byte MaxPrefixLength = 10;
    public const byte MaxReminders = 10;
    public const short MinDelayBetweenMessages = 1300;
    public const string Suffix = "eg";

    public static List<string> Channels => DatabaseController.GetChannels();

    public static List<string> Owners => new JsonController().Settings.UserLists.Owners;

    public static List<string> Moderators => new JsonController().Settings.UserLists.Moderators.Concat(Owners).Distinct().ToList();

    public static List<string> SpecialUsers => new JsonController().Settings.UserLists.IgnoredUsers;

    public static List<string> SecretUsers => new JsonController().Settings.UserLists.SecretUsers;

    public static List<string> NotLoggedChannels => Settings.NotLoggedChannels.Split().ToList();
}
