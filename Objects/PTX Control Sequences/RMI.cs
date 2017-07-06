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
        public int Increment { get; private set; }

        public RMI(byte id, byte[] sequence, byte[] data) : base(id, sequence, data) { }

        public override void ParseData()
        {
            Increment = (int)GetNumericValue(GetSectionedData(0, 2), true);
        }
    }
}