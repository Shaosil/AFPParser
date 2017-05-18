using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class Comment : Triplet
	{
		private static string _desc = "Used to include comments for documentation purposes.";

		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>()
		{
			new Offset(2, Lookups.DataTypes.CHAR, "Comment")
		};

		public Comment(byte[] allData) : base(allData) { }
	}
}