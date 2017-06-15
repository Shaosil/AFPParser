using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class ResourceLocalIdentifier : Triplet
	{
		private static string _desc = "Specifies a resource type and one byte local identifier (LID).";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "Resource Type")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Usage dependant" },
                    { 0x02, "Page Overlay" },
                    { 0x05, "Coded Font" }
                }
            },
            new Offset(1, Lookups.DataTypes.EMPTY, "Resource LID")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public ResourceLocalIdentifier(byte[] allData) : base(allData) { }
	}
}