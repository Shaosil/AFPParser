using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class UNKNOWN : StructuredField
    {
        public override string Abbreviation { get { return "UNK"; } }

        public override string Title { get { return "UNKNOWN"; } }

        public override string Description { get { return $"UNKNOWN HEX CODE: {HexID}"; } }

        protected override bool IsRepeatingGroup { get { return false; } }

        public override IReadOnlyList<Offset> Offsets { get { return new List<Offset>(); } }

        protected override int RepeatingGroupStart { get { return 0; } }

        public UNKNOWN(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
    }
}
