using System.Collections.Generic;

namespace AFPParser.ImageSelfDefiningFields
{
    public class EndImageContent : ImageSelfDefiningField
    {
        private static string _desc = "";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public EndImageContent(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
    }
}