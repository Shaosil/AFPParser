using System.Collections.Generic;

namespace AFPParser
{
    public class SemanticsInfo
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public bool IsRepeatingGroup { get; private set; }
        public int RepeatingGroupStart { get; private set; }
        public IReadOnlyList<Offset> Offsets { get; private set; }

        public SemanticsInfo(string title, string description, bool isRg, int rgStart, List<Offset> offsets)
        {
            Title = title;
            Description = description;
            IsRepeatingGroup = isRg;
            RepeatingGroupStart = rgStart;
            Offsets = offsets;
        }
    }
}
