using System.Collections.Generic;
using System.Text;

namespace AFPParser.StructuredFields
{
    public class PTD2 : StructuredField
    {
        private static string _abbr = "PTD";
        private static string _title = "Presentation Text Descriptor (Format 2)";
        private static string _desc = "The Presentation Text Data Descriptor structured field contains the descriptor data for a presentation text data object.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "X Axis Base") { Mappings = CommonMappings.UnitBase },
            new Offset(1, Lookups.DataTypes.CODE, "Y Axis Base") { Mappings = CommonMappings.UnitBase },
            new Offset(2, Lookups.DataTypes.UBIN, "Xp Units per Base"),
            new Offset(4, Lookups.DataTypes.UBIN, "Yp Units per Base"),
            new Offset(6, Lookups.DataTypes.UBIN, "Xp Extent"),
            new Offset(9, Lookups.DataTypes.UBIN, "Yp Extent"),
            new Offset(12, Lookups.DataTypes.EMPTY, ""),
            new Offset(14, Lookups.DataTypes.EMPTY, "Initial Text Condition Chain - CUSTOM PARSED")
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

        public PTD2(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            if (oSet.StartingIndex != 14)
                return base.GetSingleOffsetDescription(oSet, sectionedData);

            StringBuilder sb = new StringBuilder();

            // This section can specify default values for control sequences. Load them up as normal sequences
            sb.AppendLine("Initial text conditions not yet implemented...");

            return sb.ToString();
        }

        public override void ParseData()
        {
            BaseUnit = (Converters.eMeasurement)Data[0];
            UnitsPerXBase = GetNumericValueFromData<ushort>(2, 2);
            UnitsPerYBase = GetNumericValueFromData<ushort>(4, 2);
            XSize = GetNumericValueFromData<uint>(6, 3);
            YSize = GetNumericValueFromData<uint>(9, 3);
        }

        public override string GetFullDescription()
        {
            StringBuilder sb = new StringBuilder(base.GetFullDescription());

            // Calculate the presentation space width/height
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("Presentation space: ");
            sb.Append($"{Converters.GetMeasurement((int)XSize, UnitsPerXBase)} x {Converters.GetMeasurement((int)YSize, UnitsPerYBase)}");
            sb.AppendLine($" {BaseUnit.ToString()}");

            return sb.ToString();
        }
    }
}