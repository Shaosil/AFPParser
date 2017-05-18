using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class MediumMapPageNumber : Triplet
	{
		private static string _desc = "Specifies the sequence number of the page in the set of pages whose presentation is controlled by the most recently activated medium map.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.UBIN, "Page number")
		};

		protected override string Description => _desc;
        protected override List<Offset> Offsets => _oSets;

		public MediumMapPageNumber(byte[] allData) : base(allData) { }
	}
}