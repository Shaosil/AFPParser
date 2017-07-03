using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace AFPParser.StructuredFields
{
	public class IOB : StructuredField
	{
		private static string _abbr = "IOB";
		private static string _title = "Include Object";
		private static string _desc = "References an object on a page or overlay. Optionally contains parameters that identify the object and specify presentation parameters.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Object Name"),
            new Offset(8, Lookups.DataTypes.EMPTY, ""),
            new Offset(9, Lookups.DataTypes.CODE, "Object Type")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x5F, "Page Segment" },
                    { 0x92, "Other Object Data" },
                    { 0xBB, "Graphics (GOCA)" },
                    { 0xEB, "Bar Code (BCOCA)" },
                    { 0xFB, "Image (IOCA)" }
                }
            },
            new Offset(10, Lookups.DataTypes.SBIN, "X Origin"), // CUSTOM PARSED
            new Offset(13, Lookups.DataTypes.SBIN, "Y Origin"), // CUSTOM PARSED
            new Offset(16, Lookups.DataTypes.CODE, "X Rotation") { Mappings = CommonMappings.Rotations }, // CUSTOM PARSED
            new Offset(18, Lookups.DataTypes.CODE, "Y Rotation") { Mappings = CommonMappings.Rotations }, // CUSTOM PARSED
            new Offset(20, Lookups.DataTypes.SBIN, "Content X Origin"), // CUSTOM PARSED
            new Offset(23, Lookups.DataTypes.SBIN, "Content Y Origin"), // CUSTOM PARSED
            new Offset(26, Lookups.DataTypes.CODE, "Reference Coordinate System") { Mappings = new Dictionary<byte, string>() { { 0x01, "Page or Overlay" } } },
            new Offset(27, Lookups.DataTypes.TRIPS, "")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public IOB(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            // For all origins and rotations, if the first two bytes are 0xFFFF, the value is defined in the object itself
            if (oSet.StartingIndex < 10 || oSet.StartingIndex > 23
            || !GetSectionedData(oSet.StartingIndex, 2).SequenceEqual(new byte[] { 0xFF, 0xFF }))
                return base.GetSingleOffsetDescription(oSet, sectionedData);

            // We're only here if it is a custom parsed offset whose first two bytes are 0xFFFF
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{oSet.Description}: Defined in Object");
            return sb.ToString();
        }
    }
}