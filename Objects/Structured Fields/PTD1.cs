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
            new Offset(0, Lookups.DataTypes.CODE, "X Axis Unit Base") { Mappings = CommonMappings.UnitBase },
            new Offset(1, Lookups.DataTypes.CODE, "Y Axis Unit Base") { Mappings = CommonMappings.UnitBase },
            new Offset(2, Lookups.DataTypes.UBIN, "Units Per X Base"),
            new Offset(4, Lookups.DataTypes.UBIN, "Units Per Y Base"),
            new Offset(6, Lookups.DataTypes.UBIN, "X Axis Space Extent"),
            new Offset(8, Lookups.DataTypes.UBIN, "Y Axis Space Extent"),
            new Offset(10, Lookups.DataTypes.EMPTY, "")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public Converters.eMeasurement BaseUnit { get; private set; }
        public int UnitsPerXBase { get; private set; }
        public int UnitsPerYBase { get; private set; }
        public int XSize { get; private set; }
        public int YSize { get; private set; }

        public PTD1(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public override void ParseData()
        {
            BaseUnit = Converters.GetBaseUnit(Data[0]);
            UnitsPerXBase = (int)GetNumericValue(GetSectionedData(2, 2), false);
            UnitsPerYBase = (int)GetNumericValue(GetSectionedData(4, 2), false);
            XSize = (int)GetNumericValue(GetSectionedData(6, 2), false);
            YSize = (int)GetNumericValue(GetSectionedData(8, 2), false);
        }

        public override string GetFullDescription()
        {
            StringBuilder sb = new StringBuilder(base.GetFullDescription());

            // Calculate the presentation space width/height
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("Presentation space: ");
            sb.Append($"{Converters.GetMeasurement(XSize, UnitsPerXBase)} x {Converters.GetMeasurement(YSize, UnitsPerYBase)}");
            sb.AppendLine($" {BaseUnit.ToString()}");

            return sb.ToString();
        }
    }
}