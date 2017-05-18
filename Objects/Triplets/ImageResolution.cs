using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class ImageResolution : Triplet
	{
		private static string _desc = "";

		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>();

		public ImageResolution(byte[] allData) : base(allData) { }
	}
}