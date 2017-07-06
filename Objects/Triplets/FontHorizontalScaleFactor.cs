using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class FontHorizontalScaleFactor : Triplet
    {
        private static string _desc = "Carries information to support anamorphic scaling of an outline technology font.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "Horizontal Scale")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public FontHorizontalScaleFactor(byte id, byte[] data) : base(id, data) { }
    }
}