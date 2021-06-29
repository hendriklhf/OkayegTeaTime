using System;
using System.Collections.Generic;

#nullable disable

namespace OkayegTeaTimeCSharp.Database.Models
{
    public partial class EmoteInFront
    {
        public int Id { get; set; }
        public string Channel { get; set; }
        public byte[] Emote { get; set; }
    }
}
