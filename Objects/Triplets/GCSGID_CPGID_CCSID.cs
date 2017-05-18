using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class GCSGID_CPGID_CCSID : Triplet
	{
		private static string _desc = "Used to establish the values of the code page/char set for all field parameters having a CHAR data type.";

		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>()
		{
			new Offset(2, Lookups.DataTypes.CODE, "GCSGID"),
			new Offset(4, Lookups.DataTypes.CODE, "CCSID")
		};

		public GCSGID_CPGID_CCSID(byte[] allData) : base(allData) { }
	}
}