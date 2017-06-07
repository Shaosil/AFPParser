using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class DescriptorPosition : Triplet
	{
		private static string _desc = "Used to associate an object area position field with an object area description field.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, "Object Area Position ID")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public DescriptorPosition(byte[] allData) : base(allData) { }
	}
}