using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AFPParser.StructuredFields
{
    public class IID : StructuredField
    {
        private static string _abbr = "IID";
        private static string _title = "Image Input Descriptor IM";
        private static string _desc = "Contains the resolution, size, and color information of the IM image.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(12, Lookups.DataTypes.CODE, "X Axis Base") { Mappings = CommonMappings.UnitBase },
            new Offset(13, Lookups.DataTypes.CODE, "Y Axis Base") { Mappings = CommonMappings.UnitBase },
            new Offset(14, Lookups.DataTypes.UBIN, "X Units per Base"),
            new Offset(16, Lookups.DataTypes.UBIN, "Y Units per Base"),
            new Offset(18, Lookups.DataTypes.UBIN, "X Size"),
            new Offset(20, Lookups.DataTypes.UBIN, "Y Size"),
            new Offset(22, Lookups.DataTypes.EMPTY, ""),
            new Offset(28, Lookups.DataTypes.UBIN, "Default X Cell Size"),
            new Offset(30, Lookups.DataTypes.UBIN, "Default Y Cell Size"),
            new Offset(32, Lookups.DataTypes.EMPTY, ""),
            new Offset(34, Lookups.DataTypes.CODE, "Color - CUSTOM PARSED")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public Converters.eMeasurement BaseUnit { get; private set; }
        public ushort XSize { get; private set; }
        public ushort YSize { get; private set; }
        public ushort XUnitsPerBase { get; private set; }
        public ushort YUnitsPerBase { get; private set; }
        public Color ImageColor { get; private set; }

        public IID(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            if (oSet.StartingIndex != 34)
                return base.GetSingleOffsetDescription(oSet, sectionedData);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ImageColor.ToString());
            return sb.ToString();
        }

        public override void ParseData()
        {
            BaseUnit = (Converters.eMeasurement)Data[12];
            XSize = GetNumericValueFromData<ushort>(18, 2);
            YSize = GetNumericValueFromData<ushort>(20, 2);
            XUnitsPerBase = GetNumericValueFromData<ushort>(14, 2);
            YUnitsPerBase = GetNumericValueFromData<ushort>(16, 2);
            ImageColor = Color.Black;
            if (Lookups.StandardOCAColors.ContainsKey(Data[35]))
                ImageColor = Lookups.StandardOCAColors[Data[35]];
        }
    }
}