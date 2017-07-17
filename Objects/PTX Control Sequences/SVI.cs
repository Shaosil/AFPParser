using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class SVI : PTXControlSequence
    {
        private static string _abbr = "SVI";
        private static string _desc = "Set Variable Space Character Increment";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.SBIN, "Increment")
        };

        public override string Abbreviation => _abbr;
        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public short Increment { get; private set; }

        public SVI(byte id, bool hasPrefix, byte[] data) : base(id, hasPrefix, data) { }

        public override void ParseData()
        {
            // Don't set if FFFF
            Increment = GetNumericValueFromData<short>(0, 2);
            if (Increment < 0) Increment = 0;
        }
    }
}