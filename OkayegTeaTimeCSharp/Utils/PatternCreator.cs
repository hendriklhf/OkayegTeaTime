using OkayegTeaTimeCSharp.Commands.CommandEnums;

namespace OkayegTeaTimeCSharp.Utils
{
    public static class PatternCreator
    {
        public static string Create(string alias, PrefixType prefixType, string addition = "")
        {
            if (prefixType.Equals(PrefixType.None))
            {
                return @"^" + alias + @"eg" + addition;
            }
            else //if (prefixType.Equals(PrefixType.Active))
            {
                return @"^\S+" + alias + addition;
            }
        }
    }
}
