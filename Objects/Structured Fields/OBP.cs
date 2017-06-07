using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class OBP : StructuredField
	{
		private static string _abbr = "OBP";
		private static string _title = "Object Area Position";
		private static string _desc = "The Object Area Position structured field specifies the origin and orientation of the object area, and the origin and orientation of the object content within the object area.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, ""), // Actually the RG Length, but it's always 23
            new Offset(2, Lookups.DataTypes.SBIN, "X Axis Area Origin"),
            new Offset(5, Lookups.DataTypes.SBIN, "Y Axis Area Origin"),
            new Offset(8, Lookups.DataTypes.CODE, "X Axis Rotation") { Mappings = Lookups.CommonMappings.Rotations },
            new Offset(10, Lookups.DataTypes.CODE, "Y Axis Rotation") { Mappings = Lookups.CommonMappings.Rotations },
            new Offset(12, Lookups.DataTypes.EMPTY, ""),
            new Offset(13, Lookups.DataTypes.SBIN, "X Axis Content Origin"),
            new Offset(16, Lookups.DataTypes.SBIN, "Y Axis Content Origin"),
            new Offset(19, Lookups.DataTypes.CODE, "X Axis Content Rotation") { Mappings = Lookups.CommonMappings.Rotations },
            new Offset(21, Lookups.DataTypes.CODE, "Y Axis Content Rotation") { Mappings = Lookups.CommonMappings.Rotations },
            new Offset(23, Lookups.DataTypes.CODE, "Reference Coordinate System")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Defined in IPS field" },
                    { 0x01, "Standard origin" }
                }
            }
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => true;
		protected override int RepeatingGroupStart => 1;
        protected override int RepeatingGroupLength => 23;
        public override IReadOnlyList<Offset> Offsets => _oSets;

		public OBP(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}