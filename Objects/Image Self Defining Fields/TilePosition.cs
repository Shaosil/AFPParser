using System.Collections.Generic;

namespace AFPParser.ImageSelfDefiningFields
{
    public class TilePosition : ImageSelfDefiningField
    {
        private static string _desc = "Determines the position of the left upper corner of the tile.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "X Offset"),
            new Offset(4, Lookups.DataTypes.UBIN, "Y Offset")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public uint XOffset { get; private set; }
        public uint YOffset { get; private set; }

        public TilePosition(byte[] id, byte[] data) : base(id, data) { }

        public override void ParseData()
        {
            XOffset = (uint)GetNumericValue(GetSectionedData(0, 4), false);
            YOffset = (uint)GetNumericValue(GetSectionedData(4, 4), false);
        }
    }
}