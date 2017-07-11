using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class AMI : PTXControlSequence
    {
        private static string _abbr = "AMI";
        private static string _desc = "Absolute Move Inline";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.SBIN, "Displacement")
        };

        public override string Abbreviation => _abbr;
        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public short Displacement { get; private set; }

        public AMI(byte id, byte[] sequence, byte[] data) : base(id, sequence, data) { }

        public override void ParseData()
        {
            Displacement = GetNumericValueFromData<short>(0, 2);
        }
    }
}