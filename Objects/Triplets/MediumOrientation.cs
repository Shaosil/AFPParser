using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class MediumOrientation : Triplet
	{
		private static string _desc = "";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public MediumOrientation(byte[] allData) : base(allData) { }
	}
}