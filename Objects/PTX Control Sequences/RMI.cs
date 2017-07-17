using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class RMI : PTXControlSequence
    {
        private static string _abbr = "RMI";
        private static string _desc = "Relative Move Inline";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.SBIN, "Increment")
        };

        public override string Abbreviation => _abbr;
        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public short Increment { get; private set; }

        public RMI(byte id, bool hasPrefix, byte[] data) : base(id, hasPrefix, data) { }

        public override void ParseData()
        {
            Increment = GetNumericValueFromData<short>(0, 2);
        }
    }
}