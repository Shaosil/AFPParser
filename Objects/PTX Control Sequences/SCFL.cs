using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class SCFL : PTXControlSequence
    {
        private static string _abbr = "SCFL";
        private static string _desc = "Set Coded Font Local";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "Identifier")
        };

        public override string Abbreviation => _abbr;
        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public byte FontId
        {
            get { return Data[0]; }
            private set { Data[0] = value; }
        }

        public SCFL(byte id, bool hasPrefix, byte[] data) : base(id, hasPrefix, data) { }

        public SCFL(byte fontId, bool hasPrefix, bool isChained) : base(Lookups.PTXControlSequenceID<SCFL>(isChained), hasPrefix, new byte[1] { fontId }) { }
    }
}