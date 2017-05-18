using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class UP3iFinishingOperation : Triplet
	{
		private static string _desc = "";
        private static List<Offset> _oSets = new List<Offset>();

        protected override string Description => _desc;
        protected override List<Offset> Offsets => _oSets;

        public UP3iFinishingOperation(byte[] allData) : base(allData) { }
	}
}