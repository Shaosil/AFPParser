using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class USC : PTXControlSequence
    {
        private static string _abbr = "USC";
        private static string _desc = "Underscore";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public USC(byte id, bool isChained, byte[] data) : base(id, isChained, data) { }
    }
}
