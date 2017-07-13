using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class RPS : PTXControlSequence
    {
        private static string _abbr = "RPS";
        private static string _desc = "Repeat String";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public RPS(byte id, bool isChained, byte[] data) : base(id, isChained, data) { }
    }
}
