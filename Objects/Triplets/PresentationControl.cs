using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class PresentationControl : Triplet
	{
		private static string _desc = "Specifies flags that control the presentation of an object.";

		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>()
		{
			new Offset(2, Lookups.DataTypes.BITS, "PRS Flag")
			{
				Mappings = new Dictionary<byte, string>()
				{
					{ 0x0, "Intended for viewing|Not intended for viewing" },
					{ 0x1, "Intended to be indexed|Not intended to be indexed" }
				}
			}
		};

		public PresentationControl(byte[] allData) : base(allData) { }
	}
}