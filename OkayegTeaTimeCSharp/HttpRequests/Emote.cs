namespace OkayegTeaTimeCSharp.HttpRequests
{
    public struct Emote
    {
        public int Index { get; }

        public string Name { get; }

        public Emote(int index, string name)
        {
            Index = index;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}