using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class PagePositionInformation : Triplet
	{
		private static string _desc = "Tags a page with the Page Position (PGP) structured field repeating group info that is used to present the page.";

		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>()
		{
			new Offset(2, Lookups.DataTypes.UBIN, "PGPRG Number")
		};

		public PagePositionInformation(byte[] allData) : base(allData) { }
	}
}