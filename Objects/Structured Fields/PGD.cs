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
                PutNumberInData(value, 7);
            }
        }
        public ushort YSize
        {
            get { return _ySize; }
            private set
            {
                _ySize = value;
                PutNumberInData(value, 10);
            }
        }

        public PGD(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public PGD(ushort unitsPerXBase, ushort unitsPerYBase, ushort xSize, ushort ySize) : base(Lookups.StructuredFieldID<PGD>(), 0, 0, null)
        {
            Data = new byte[15];
            BaseUnit = Converters.eMeasurement.Inches;
            UnitsPerXBase = unitsPerXBase;
            UnitsPerYBase = unitsPerYBase;
            XSize = xSize;
            YSize = ySize;
        }

        public override void ParseData()
        {
            base.ParseData();

            _baseUnit = (Converters.eMeasurement)Data[0];
            _unitsPerXBase = GetNumericValueFromData<ushort>(2, 2);
            _unitsPerYBase = GetNumericValueFromData<ushort>(4, 2);
            _xSize = GetNumericValueFromData<ushort>(7, 2);
            _ySize = GetNumericValueFromData<ushort>(10, 2);
        }

        public override string GetFullDescription()
        {
            StringBuilder sb = new StringBuilder(base.GetFullDescription());

            // Calculate the page width/height
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("Page size: ");
            sb.Append($"{Converters.GetMeasurement(XSize, UnitsPerXBase)} x {Converters.GetMeasurement(YSize, UnitsPerYBase)}");
            sb.AppendLine($" {BaseUnit.ToString()}");

            return sb.ToString();
        }
    }
}