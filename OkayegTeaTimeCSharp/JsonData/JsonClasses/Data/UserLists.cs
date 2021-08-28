using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.Data
{
    public class UserLists
    {
        public List<string> Owners { get; set; }

        public List<string> Moderators { get; set; }

        public List<string> SpecialUsers { get; set; }

        public List<string> SecretUsers { get; set; }
    }
}
