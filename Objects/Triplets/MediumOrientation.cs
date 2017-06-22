using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class MediumOrientation : Triplet
	{
		private static string _desc = "Specifies the orientation of the medium presentation space on the physical medium.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "Orientation")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Portrait" },
                    { 0x01, "Landscape" },
                    { 0x02, "Reverse Portrait" },
                    { 0x03, "Reverse Landscape" },
                    { 0x04, "Portrait 90" },
                    { 0x05, "Landscape 90" },
                }
            }
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public MediumOrientation(byte[] allData) : base(allData) { }
	}
}