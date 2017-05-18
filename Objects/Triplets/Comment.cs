using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class Comment : Triplet
	{
		private static string _desc = "Used to include comments for documentation purposes.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Comment")
        };

        protected override string Description => _desc;
        protected override List<Offset> Offsets => _oSets;

		public Comment(byte[] allData) : base(allData) { }
	}
}