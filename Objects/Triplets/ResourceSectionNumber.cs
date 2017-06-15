using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class ResourceSectionNumber : Triplet
	{
		private static string _desc = "Specifies a coded font section number.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "Resource Section Number")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public ResourceSectionNumber(byte[] allData) : base(allData) { }
	}
}