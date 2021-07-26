using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkayegTeaTimeCSharp.JsonData.JsonClasses.CommandData
{
    public class Command
    {
        public string CommandName { get; set; }

        public List<string> Alias { get; set; }

        public List<string> Parameter { get; set; }

        public List<string> Description { get; set; }

        public int Cooldown { get; set; }
    }
}
