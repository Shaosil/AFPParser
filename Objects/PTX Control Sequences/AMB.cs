using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class AMB : PTXControlSequence
    {
        private static string _abbr = "AMB";
        private static string _desc = "Absolute Move Baseline";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.SBIN, "Displacement")
        };

        public override string Abbreviation => _abbr;
        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public short Displacement { get; private set; }

        public AMB(byte id, byte[] sequence, byte[] data) : base(id, sequence, data) { }

        public override void ParseData()
        {
            Displacement = GetNumericValueFromData<short>(0, 2);
        }
    }
}