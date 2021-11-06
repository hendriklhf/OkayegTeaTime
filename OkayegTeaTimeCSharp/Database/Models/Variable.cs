using System;
using System.Collections.Generic;

#nullable disable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class Variable
    {
        public string Name { get; set; }
        public byte[] Value { get; set; }
    }
}
