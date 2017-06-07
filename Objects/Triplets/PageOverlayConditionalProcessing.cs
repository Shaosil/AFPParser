using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class PageOverlayConditionalProcessing : Triplet
	{
		private static string _desc = "";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public PageOverlayConditionalProcessing(byte[] allData) : base(allData) { }
	}
}