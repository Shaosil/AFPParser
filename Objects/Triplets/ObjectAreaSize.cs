using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class ObjectAreaSize : Triplet
	{
		private static string _desc = "Specifies the extent of an object area in the X and Y directions.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "Size Type")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x02, "Object Area Size" }
                }
            },
            new Offset(1, Lookups.DataTypes.UBIN, "X Axis Extent"),
            new Offset(4, Lookups.DataTypes.UBIN, "Y Axis Extent")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public int XExtent { get; private set; }
        public int YExtent { get; private set; }

        public ObjectAreaSize(byte id, byte[] introducer, byte[] data) : base(id, introducer, data) { }

        public override void ParseData()
        {
            XExtent = (int)GetNumericValue(GetSectionedData(1, 3), false);
            YExtent = (int)GetNumericValue(GetSectionedData(4, 3), false);
        }
    }
}