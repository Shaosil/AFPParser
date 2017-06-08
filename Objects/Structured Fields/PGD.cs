using System.Text;
using System.Collections.Generic;
using System;

namespace AFPParser.StructuredFields
{
    public class PGD : StructuredField
    {
        private static string _abbr = "PGD";
        private static string _title = "Page Descriptor";
        private static string _desc = "Specifies the size and attributes of a page or overlay presentation space.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "X Axis Base") { Mappings = Lookups.CommonMappings.AxisBase },
            new Offset(1, Lookups.DataTypes.CODE, "Y Axis Base") { Mappings = Lookups.CommonMappings.AxisBase },
            new Offset(2, Lookups.DataTypes.UBIN, "Units Per X Base"),
            new Offset(4, Lookups.DataTypes.UBIN, "Units Per Y Base"),
            new Offset(6, Lookups.DataTypes.UBIN, "Page X Size"),
            new Offset(9, Lookups.DataTypes.UBIN, "Page Y Size"),
            new Offset(12, Lookups.DataTypes.EMPTY, ""),
            new Offset(15, Lookups.DataTypes.TRIPS, "")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public enum eMeasurement { Inches, Centimeters }
        public eMeasurement BaseUnit { get; private set; }
        public int UnitsPerXBase { get; private set; }
        public int UnitsPerYBase { get; private set; }
        public int XSize { get; private set; }
        public int YSize { get; private set; }
        public double XInches
        {
            get
            {
                double units = Math.Round(XSize / (UnitsPerXBase / 10.0), 2);
                if (BaseUnit != eMeasurement.Inches) units *= 2.54;
                return units;
            }
        }
        public double YInches
        {
            get
            {
                double units = Math.Round(YSize / (UnitsPerYBase / 10.0), 2);
                if (BaseUnit != eMeasurement.Inches) units *= 2.54;
                return units;
            }
        }
        public double XCentimeters
        {
            get
            {
                double units = Math.Round(XSize / (UnitsPerXBase / 10.0), 2);
                if (BaseUnit != eMeasurement.Centimeters) units /= 2.54;
                return units;
            }
        }
        public double YCentimeters
        {
            get
            {
                double units = Math.Round(YSize / (UnitsPerYBase / 10.0), 2);
                if (BaseUnit != eMeasurement.Centimeters) units /= 2.54;
                return units;
            }
        }

        public PGD(int length, string hex, byte flag, int sequence) : base(length, hex, flag, sequence) { }

        public override void ParseData()
        {
            base.ParseData();

            BaseUnit = Lookups.CommonMappings.AxisBase[Data[0]] == Lookups.CommonMappings.AxisBase[0] ? eMeasurement.Inches : eMeasurement.Centimeters;
            UnitsPerXBase = (int)GetNumericValue(GetSectionedData(2, 2), false);
            UnitsPerYBase = (int)GetNumericValue(GetSectionedData(4, 2), false);
            XSize = (int)GetNumericValue(GetSectionedData(6, 3), false);
            YSize = (int)GetNumericValue(GetSectionedData(9, 3), false);
        }

        public override string GetFullDescription()
        {
            StringBuilder sb = new StringBuilder(base.GetFullDescription());

            // Calculate the page width/height
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("Page size: ");
            if (BaseUnit == eMeasurement.Inches)
                sb.Append($"{XInches} x {YInches}");
            else
                sb.Append($"{XCentimeters} x {YCentimeters}");
            sb.AppendLine($" {BaseUnit.ToString()}");

            return sb.ToString();
        }
    }
}