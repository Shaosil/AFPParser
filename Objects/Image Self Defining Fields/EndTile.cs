using System.Collections.Generic;

namespace AFPParser.ImageSelfDefiningFields
{
    public class EndTile : ImageSelfDefiningField
    {
        private static string _desc = "";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public EndTile(int paramLength, string id, byte[] data) : base(paramLength, id, data) { }
    }
}