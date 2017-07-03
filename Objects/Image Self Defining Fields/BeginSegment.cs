using System.Collections.Generic;

namespace AFPParser.ImageSelfDefiningFields
{
    public class BeginSegment : ImageSelfDefiningField
    {
        private static string _desc = "Defines the beginning of the Image Segment.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "Image Segment ID")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public BeginSegment(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
    }
}