using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class PresentationControl : Triplet
	{
		private static string _desc = "Specifies flags that control the presentation of an object.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.BITS, "PRS Flag")
			{
				Mappings = new Dictionary<byte, string>()
				{
					{ 0x0, "Intended for viewing|Not intended for viewing" },
					{ 0x1, "Intended to be indexed|Not intended to be indexed" }
				}
			}
		};

		public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

		public PresentationControl(byte[] allData) : base(allData) { }
	}
}