using System.Collections.Generic;

namespace AFPParser.ImageSelfDefiningFields
{
    public class UNKNOWN : ImageSelfDefiningField
    {
        private static string _desc = "UNKNOWN";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public UNKNOWN(int paramLength, string id, byte[] data) : base(paramLength, id, data) { }
    }
}