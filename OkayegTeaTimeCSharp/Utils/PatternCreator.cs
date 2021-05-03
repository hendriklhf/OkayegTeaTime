using OkayegTeaTimeCSharp.Commands.CommandEnums;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class PatternCreator
    {
        public const string ActivePrefixEnding = @"(\s|$)";
        public const string NonePrefixEnding = @"eg(\s|$)";

        public static string Create(string alias, PrefixType prefixType, string addition = "")
        {
            return prefixType.Equals(PrefixType.Active) ? @"^\S{1,10}" + alias + addition : @"^" + alias + @"eg" + addition;
        }
    }
}
