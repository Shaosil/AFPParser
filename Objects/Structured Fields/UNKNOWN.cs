using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class UNKNOWN : StructuredField
    {
        public override string Abbreviation { get { return "UNK"; } }

        public override string Title { get { return "UNKNOWN"; } }

        protected override string Description { get { return $"UNKNOWN HEX CODE: {ID}"; } }

        protected override bool IsRepeatingGroup { get { return false; } }

        protected override List<Offset> Offsets { get { return new List<Offset>(); } }

        protected override int RepeatingGroupStart { get { return 0; } }

        public UNKNOWN(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
    }
}
