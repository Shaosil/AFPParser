using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class DIR : PTXControlSequence
    {
        private static string _abbr = "DIR";
        private static string _desc = "Draw I-axis Rule";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.SBIN, "Length"),
            new Offset(2, Lookups.DataTypes.SBIN, "Width"),
            new Offset(4, Lookups.DataTypes.EMPTY, "") // Actually the frational specifier byte of the width, but we probably don't need to support that
        };

        public override string Abbreviation => _abbr;
        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public short RuleLength { get; private set; }
        public short RuleWidth { get; private set; }

        public DIR(byte id, bool hasPrefix, byte[] data) : base(id, hasPrefix, data) { }

        public override void ParseData()
        {
            RuleLength = GetNumericValueFromData<short>(0, 2);
            RuleWidth = GetNumericValueFromData<short>(2, 2);
        }
    }
}
