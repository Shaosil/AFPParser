using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class SIM : PTXControlSequence
    {
        private static string _abbr = "SIM";
        private static string _desc = "Set Inline Margin";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public SIM(byte id, bool hasPrefix, byte[] data) : base(id, hasPrefix, data) { }
    }
}