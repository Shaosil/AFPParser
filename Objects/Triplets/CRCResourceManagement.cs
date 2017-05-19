using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class CRCResourceManagement : Triplet
	{
		private static string _desc = "Provides resource management information such as a public/private flag and a retired Cyclic Redundancy Check value.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, ""),
            new Offset(1, Lookups.DataTypes.UBIN, ""),
            new Offset(3, Lookups.DataTypes.BITS, "Resource Class Flag")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Contains no privately owned information|Contains privately owned information" }
                }
            }
        };

        protected override string Description => _desc;
        protected override List<Offset> Offsets => _oSets;

        public CRCResourceManagement(byte[] allData) : base(allData) { }
	}
}