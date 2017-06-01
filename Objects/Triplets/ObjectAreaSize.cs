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

        protected override string Description => _desc;
        protected override List<Offset> Offsets => _oSets;

        public ObjectAreaSize(byte[] allData) : base(allData) { }
	}
}