using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class ObjectAreaSize : Triplet
	{
		private static string _desc = "";

		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public ObjectAreaSize(byte[] allData) : base(allData) { }
	}
}