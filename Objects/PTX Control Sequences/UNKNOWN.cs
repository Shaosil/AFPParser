using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class UNKNOWN : PTXControlSequence
    {
        public override string Abbreviation { get { return "UNK"; } }
        public override string Description { get { return $"UNKNOWN CONTROL SEQUENCE: {HexIDStr}"; } }

        public override IReadOnlyList<Offset> Offsets { get { return new List<Offset>(); } }

        public UNKNOWN(byte id, bool hasPrefix, byte[] data) : base(id, hasPrefix, data) { }
    }
}