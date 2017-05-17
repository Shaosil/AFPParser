using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class UNKNOWN : PTXControlSequence
    {
        public override string Abbreviation { get { return "UNK"; } }
        protected override string Description { get { return $"UNKNOWN CONTROL SEQUENCE: {ID}"; } }

        protected override List<Offset> Offsets { get { return new List<Offset>(); } }

        public UNKNOWN(byte[] allData) : base (allData) { }
    }
}