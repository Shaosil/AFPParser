using System.Collections.Generic;

namespace AFPParser.ImageSelfDefiningFields
{
    public class ImageLUT_ID : ImageSelfDefiningField
    {
        private static string _desc = "";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public ImageLUT_ID(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
    }
}