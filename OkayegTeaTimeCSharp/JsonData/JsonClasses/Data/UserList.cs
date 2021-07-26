using System.Collections.Generic;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.Data
{
    public class UserList
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public List<string> Matches { get; set; }
    }
}