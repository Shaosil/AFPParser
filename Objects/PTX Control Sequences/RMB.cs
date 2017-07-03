using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class RMB : PTXControlSequence
    {
        private static string _abbr = "RMB";
        private static string _desc = "Relative Move Baseline";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.SBIN, "Increment")
        };

        public override string Abbreviation => _abbr;
        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public int Increment { get; private set; }

        public RMB(byte id, byte[] introducer, byte[] data) : base(id, introducer, data) { }

        public override void ParseData()
        {
            Increment = (int)GetNumericValue(GetSectionedData(0, 2), true);
        }
    }
}