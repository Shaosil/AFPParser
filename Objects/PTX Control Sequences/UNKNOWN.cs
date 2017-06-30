using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class UNKNOWN : PTXControlSequence
    {
        public override string Abbreviation { get { return "UNK"; } }
        public override string Description { get { return $"UNKNOWN CONTROL SEQUENCE: {HexID}"; } }

        public override IReadOnlyList<Offset> Offsets { get { return new List<Offset>(); } }

        public UNKNOWN(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
    }
}