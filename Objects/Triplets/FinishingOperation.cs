using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class FinishingOperation : Triplet
    {
        private static string _desc = "";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public FinishingOperation(byte id, byte[] data) : base(id, data) { }
    }
}