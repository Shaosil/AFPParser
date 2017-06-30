using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class IOC : StructuredField
	{
		private static string _abbr = "IOC";
		private static string _title = "Image Output Control IM";
		private static string _desc = "The IM Image Output Control structured field specifies the position and orientation of the IM image object area and the mapping of the image points to presentation device pels.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "X Axis Origin"),
            new Offset(3, Lookups.DataTypes.UBIN, "Y Axis Origin"),
            new Offset(6, Lookups.DataTypes.CODE, "X Axis Rotation") { Mappings = CommonMappings.Rotations },
            new Offset(8, Lookups.DataTypes.CODE, "Y Axis Rotation") { Mappings = CommonMappings.Rotations },
            new Offset(18, Lookups.DataTypes.CODE, "Image Mapping - CUSTOM PARSED")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public IOC(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            if (oSet.StartingIndex != 18)
                return base.GetSingleOffsetDescription(oSet, sectionedData);

            StringBuilder sb = new StringBuilder();
            byte[] twoPel = new byte[2] { 0x07, 0xD0 };
            string xMapping = string.Empty, yMapping = string.Empty;
            if (GetSectionedData(18, 2).SequenceEqual(twoPel)) xMapping = "two ";
            if (GetSectionedData(20, 2).SequenceEqual(twoPel)) yMapping = "two ";

            sb.AppendLine($"X Image Mapping: Point-to-{xMapping}pel");
            sb.AppendLine($"Y Image Mapping: Point-to-{yMapping}pel");

            return sb.ToString();
        }
    }
}