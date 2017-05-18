using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class PagePositionInformation : Triplet
	{
		private static string _desc = "Tags a page with the Page Position (PGP) structured field repeating group info that is used to present the page.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.UBIN, "PGPRG Number")
		};

		protected override string Description => _desc;
        protected override List<Offset> Offsets => _oSets;

		public PagePositionInformation(byte[] allData) : base(allData) { }
	}
}