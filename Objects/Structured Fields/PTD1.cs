using System;
using System.Text;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class PTD1 : StructuredField
	{
		private static string _abbr = "PTD";
		private static string _title = "Presentation Text Descriptor (Format 1)";
		private static string _desc = "Specifies the size of a text object presentation space and the measurement units used for size and for all linear measurements within the text object.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.CODE, "X Axis Unit Base") { Mappings = Lookups.CommonMappings.AxisBase },
			new Offset(1, Lookups.DataTypes.CODE, "Y Axis Unit Base") { Mappings = Lookups.CommonMappings.AxisBase },
			new Offset(2, Lookups.DataTypes.UBIN, "Units Per X Base"),
			new Offset(4, Lookups.DataTypes.UBIN, "Units Per Y Base"),
			new Offset(6, Lookups.DataTypes.UBIN, "X Axis Space Extent"),
			new Offset(8, Lookups.DataTypes.UBIN, "Y Axis Space Extent"),
			new Offset(10, Lookups.DataTypes.EMPTY, "")
		};

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

        // Parsed Data
        public string BaseUnit { get; private set; }
        public int UnitsPerXBase { get; private set; }
        public int UnitsPerYBase { get; private set; }
        public int XSize { get; private set; }
        public int YSize { get; private set; }

        public PTD1(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }

        public override void ParseData()
        {
            BaseUnit = Lookups.CommonMappings.AxisBase[Data[0]] == Lookups.CommonMappings.AxisBase[0] ? "Inches" : "Centimeters";
            UnitsPerXBase = (int)GetNumericValue(GetSectionedData(2, 2), false);
            UnitsPerYBase = (int)GetNumericValue(GetSectionedData(4, 2), false);
            XSize = (int)GetNumericValue(GetSectionedData(6, 2), false);
            YSize = (int)GetNumericValue(GetSectionedData(8, 2), false);
        }

        public override string GetFullDescription()
        {
            StringBuilder sb = new StringBuilder(base.GetFullDescription());

            // Calculate the PTD width/height
            double width = Math.Round(XSize / (UnitsPerXBase / 10.0), 2);
            double height = Math.Round(YSize / (UnitsPerYBase / 10.0), 2);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"Presentation space: {width} x {height} {BaseUnit}.");

            return sb.ToString();
        }
    }
}