using System.Collections.Generic;
using System.Text;

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
        private Converters.eMeasurement _baseUnit;
        private ushort _unitsPerXBase;
        private ushort _unitsPerYBase;
        private ushort _xSize;
        private ushort _ySize;

        public Converters.eMeasurement BaseUnit
        {
            get { return _baseUnit; }
            private set
            {
                _baseUnit = value;

                // Sync both X and Y unit bases
                Data[0] = (byte)value;
                Data[1] = (byte)value;
            }
        }
        public ushort UnitsPerXBase
        {
            get { return _unitsPerXBase; }
            private set
            {
                _unitsPerXBase = value;
                PutNumberInData(value, 2);
            }
        }
        public ushort UnitsPerYBase
        {
            get { return _unitsPerYBase; }
            private set
            {
                _unitsPerYBase = value;
                PutNumberInData(value, 4);
            }
        }
        public ushort XSize
        {
            get { return _xSize; }
            private set
            {
                _xSize = value;
                PutNumberInData(value, 6);
            }
        }
        public ushort YSize
        {
            get { return _ySize; }
            private set
            {
                _ySize = value;
                PutNumberInData(value, 8);
            }
        }

        public PTD1(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public PTD1(ushort unitsPerXBase, ushort unitsPerYBase, ushort xSize, ushort ySize) : base(Lookups.StructuredFieldID<PTD1>(), 0, 0, null)
        {
            Data = new byte[10];
            BaseUnit = Converters.eMeasurement.Inches;
            UnitsPerXBase = unitsPerXBase;
            UnitsPerYBase = unitsPerYBase;
            XSize = xSize;
            YSize = ySize;
        }

        public override void ParseData()
        {
            _baseUnit = (Converters.eMeasurement)Data[0];
            _unitsPerXBase = GetNumericValueFromData<ushort>(2, 2);
            _unitsPerYBase = GetNumericValueFromData<ushort>(4, 2);
            _xSize = GetNumericValueFromData<ushort>(6, 2);
            _ySize = GetNumericValueFromData<ushort>(8, 2);
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