using System.Collections.Generic;
using System.Text;

namespace AFPParser.StructuredFields
{
    public class PGD : StructuredField
    {
        private static string _abbr = "PGD";
        private static string _title = "Page Descriptor";
        private static string _desc = "Specifies the size and attributes of a page or overlay presentation space.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "X Axis Base") { Mappings = CommonMappings.UnitBase },
            new Offset(1, Lookups.DataTypes.CODE, "Y Axis Base") { Mappings = CommonMappings.UnitBase },
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
        public Converters.eMeasurement BaseUnit { get; private set; }
        public ushort UnitsPerXBase { get; private set; }
        public ushort UnitsPerYBase { get; private set; }
        public uint XSize { get; private set; }
        public uint YSize { get; private set; }

        public PGD(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public override void ParseData()
        {
            base.ParseData();

            BaseUnit = Converters.GetBaseUnit(Data[0]);
            UnitsPerXBase = GetNumericValueFromData<ushort>(2, 2);
            UnitsPerYBase = GetNumericValueFromData<ushort>(4, 2);
            XSize = GetNumericValueFromData<uint>(6, 3);
            YSize = GetNumericValueFromData<uint>(9, 3);
        }

        public override string GetFullDescription()
        {
            StringBuilder sb = new StringBuilder(base.GetFullDescription());

            // Calculate the page width/height
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("Page size: ");
            sb.Append($"{Converters.GetMeasurement((int)XSize, UnitsPerXBase)} x {Converters.GetMeasurement((int)YSize, UnitsPerYBase)}");
            sb.AppendLine($" {BaseUnit.ToString()}");

            return sb.ToString();
        }
    }
}