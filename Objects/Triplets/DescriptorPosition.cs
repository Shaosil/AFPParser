using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class DescriptorPosition : Triplet
	{
		private static string _desc = "";

		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public DescriptorPosition(byte[] allData) : base(allData) { }
	}
}