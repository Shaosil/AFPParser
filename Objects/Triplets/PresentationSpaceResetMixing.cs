using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class PresentationSpaceResetMixing : Triplet
	{
		private static string _desc = "Specifies the resulting appearance when data in a new presentation space is merged with data in an exising one.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.BITS, "Background Mixing Flags")
			{
				Mappings = new Dictionary<byte, string>()
				{
					{ 0x0, "Do NOT reset the color of the medium before placing data|Reset the color of the medium before placing data" }
				}
			}
		};

		public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

		public PresentationSpaceResetMixing(byte id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}