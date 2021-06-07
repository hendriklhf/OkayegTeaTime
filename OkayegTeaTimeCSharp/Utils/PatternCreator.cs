using OkayegTeaTimeCSharp.Commands.CommandEnums;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class PatternCreator
    {
        public static string Create(string alias, PrefixType prefixType, string prefix, string addition = "")
        {
            return prefixType.Equals(PrefixType.Active) ? @"^" + prefix + alias + addition : @"^" + alias + @"eg" + addition;
        }

        public static string CreateBoth(string alias, string prefix, string addition = "")
        {
            return @"((^" + prefix + alias + addition + @")|(^" + alias + @"eg" + addition + @"))";
        }
    }
}