using System;

namespace OkayegTeaTimeCSharp.Commands.AfkCommands
{
#warning braucht klasse die erbt
    public class Enumeration : IComparable
    {
        public string Name { get; private set; }

        public AfkMessage Message { get; private set; }

        protected Enumeration(string name, AfkMessage message)
        {
            Name = name;
            Message = message;
        }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(((Enumeration)obj).Name);
        }
    }
}
