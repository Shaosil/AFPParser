using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class IDD : StructuredField
	{
		private static string _abbr = "IDD";
		private static string _title = "Image Data Descriptor";
		private static string _desc = "Carries the parameters that define the size and resolution of the Image Presentation Space, and the parameters required to interpret the Image Segment.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "Unit Base") { Mappings = Lookups.CommonMappings.AxisBase },
            new Offset(1, Lookups.DataTypes.UBIN, "Horizontal Resolution"),
            new Offset(3, Lookups.DataTypes.UBIN, "Vertical Resolution"),
            new Offset(5, Lookups.DataTypes.UBIN, "Horizontal Size"),
            new Offset(7, Lookups.DataTypes.UBIN, "Vertical Size"),
            new Offset(9, Lookups.DataTypes.IMAGESDFS, "")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

        // Parsed Data
        public IReadOnlyList<ImageSelfDefiningField> SDFs { get; private set; }

		public IDD(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }

        public override void ParseData()
        {
            // Since there is one IDD per image object container, it is efficient enough to load them at this point
            SDFs = ImageSelfDefiningField.GetAllSDFs(Data.Skip(9).ToArray());
        }
    }
}