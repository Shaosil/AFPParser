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

        protected override string Description => _desc;
        protected override List<Offset> Offsets => _oSets;

        public BeginSegment(int paramLength, string id, byte[] data) : base(paramLength, id, data) { }
    }
}