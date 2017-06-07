using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class GCSGID_CPGID_CCSID : Triplet
	{
		private static string _desc = "Used to establish the values of the code page/char set for all field parameters having a CHAR data type.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "GCSGID"),
            new Offset(2, Lookups.DataTypes.CODE, "CCSID")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

		public GCSGID_CPGID_CCSID(byte[] allData) : base(allData) { }
	}
}