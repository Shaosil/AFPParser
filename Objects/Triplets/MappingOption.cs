using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class MappingOption : Triplet
	{
		private static string _desc = "Specifies the mappings of a data object presentation space to an object area.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "Map Value")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Position" },
                    { 0x10, "Position and Trim" },
                    { 0x20, "Scale to Fit" },
                    { 0x30, "Center and Trim" },
                    { 0x41, "Migration Mapping" },
                    { 0x42, "Migration Mapping" },
                    { 0x50, "Migration Mapping" },
                    { 0x60, "Scale to Fill" },
                    { 0x70, "UP3i Print Data Mapping" }
                }
            }
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public MappingOption(byte id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}