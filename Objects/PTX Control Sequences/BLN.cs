using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class BLN : PTXControlSequence
    {
        private static string _abbr = "BLN";
        private static string _desc = "Begin Line";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public BLN(byte id, bool isChained, byte[] data) : base(id, isChained, data) { }
    }
}
