using OkayegTeaTimeCSharp.Commands.CommandEnums;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class PatternCreator
    {
        public static string Create(string alias, PrefixType prefixType, string addition = "")
        {
            return prefixType.Equals(PrefixType.Active) ? @"^\S{1,10}" + alias + addition : @"^" + alias + @"eg" + addition;
        }

        public static string CreateBoth(string alias, string addition = "")
        {
            return @"((^\S{1,10}" + alias + addition + @")|(^" + alias + @"eg" + addition + @"))";
        }
    }
}