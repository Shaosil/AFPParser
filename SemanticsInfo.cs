using System.Collections.Generic;

namespace AFPParser
{
    public class SemanticsInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Offset> Offsets { get; set; }

        public bool IsRepeatingGroup { get; set; }
        public int RepeatingGroupStart { get; set; }

        public SemanticsInfo()
        {
            Offsets = new List<Offset>();
        }
    }
}
