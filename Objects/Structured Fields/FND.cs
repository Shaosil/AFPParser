using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class FND : StructuredField
    {
        private static string _abbr = "FND";
        private static string _title = "Font Descriptor";
        private static string _desc = "Specifies the overall characteristics of a font character set.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Typeface Description"),
            new Offset(32, Lookups.DataTypes.CODE, "Weight Class") { Mappings = CommonMappings.WeightClass },
            new Offset(33, Lookups.DataTypes.CODE, "Width Class") { Mappings = CommonMappings.WidthClass },
            new Offset(34, Lookups.DataTypes.UBIN, "Max Vertical Size"),
            new Offset(36, Lookups.DataTypes.UBIN, "Nominal Vertical Size"),
            new Offset(38, Lookups.DataTypes.UBIN, "Min Vertical Size"),
            new Offset(40, Lookups.DataTypes.UBIN, "Max Horizontal Size"),
            new Offset(42, Lookups.DataTypes.UBIN, "Nominal Horizontal Size"),
            new Offset(44, Lookups.DataTypes.UBIN, "Min Horizontal Size"),
            new Offset(46, Lookups.DataTypes.UBIN, "Design General Class"),
            new Offset(47, Lookups.DataTypes.UBIN, "Design Subclass"),
            new Offset(48, Lookups.DataTypes.UBIN, "Design Specific Group"),
            new Offset(49, Lookups.DataTypes.EMPTY, ""),
            new Offset(64, Lookups.DataTypes.BITS, "Font Design Flags") { Mappings = CommonMappings.FontDesignFlags },
            new Offset(66, Lookups.DataTypes.EMPTY, ""),
            new Offset(76, Lookups.DataTypes.UBIN, "Graphic Character Set GID"),
            new Offset(78, Lookups.DataTypes.UBIN, "Font Typeface GID"),
            new Offset(80, Lookups.DataTypes.TRIPS, ""),
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        private string _typefaceDesc;
        private CommonMappings.eWeightClass _weightClass;
        private CommonMappings.eWidthClass _widthClass;
        private ushort _maxVerticalSize;
        private ushort _nominalVerticalSize;
        private ushort _minVerticalSize;
        private ushort _maxHorizontalSize;
        private ushort _nominalHorizontalSize;
        private ushort _minHorizontalSize;
        private CommonMappings.eFontDesignFlags _fontDesignFlags;

        public string TypefaceDescription
        {
            get { return _typefaceDesc; }
            set
            {
                _typefaceDesc = value;
                PutStringInData(_typefaceDesc, 0, 32);
            }
        }
        public CommonMappings.eWeightClass WeightClass
        {
            get { return _weightClass; }
            set
            {
                _weightClass = value;
                Data[32] = (byte)_weightClass;
            }
        }
        public CommonMappings.eWidthClass WidthClass
        {
            get { return _widthClass; }
            set
            {
                _widthClass = value;
                Data[33] = (byte)_widthClass;
            }
        }
        public ushort MaxVerticalSize
        {
            get { return _maxVerticalSize; }
            set
            {
                _maxVerticalSize = value;
                PutNumberInData(value, 34);
            }
        }
        public ushort NominalVerticalSize
        {
            get { return _nominalVerticalSize; }
            set
            {
                _nominalVerticalSize = value;
                PutNumberInData(value, 36);
            }
        }
        public ushort MinVerticalSize
        {
            get { return _minVerticalSize; }
            set
            {
                _minVerticalSize = value;
                PutNumberInData(value, 38);
            }
        }
        public ushort MaxHorizontalSize
        {
            get { return _maxHorizontalSize; }
            set
            {
                _maxHorizontalSize = value;
                PutNumberInData(value, 40);
            }
        }
        public ushort NominalHorizontalSize
        {
            get { return _nominalHorizontalSize; }
            set
            {
                _nominalHorizontalSize = value;
                PutNumberInData(value, 42);
            }
        }
        public ushort MinHorizontalSize
        {
            get { return _minHorizontalSize; }
            set
            {
                _minHorizontalSize = value;
                PutNumberInData(value, 44);
            }
        }
        public CommonMappings.eFontDesignFlags FontDesignFlags
        {
            get { return _fontDesignFlags; }
            set
            {
                _fontDesignFlags = value;
                PutFlagsInData(value, CommonMappings.FontDesignFlags, 64, 2);
            }
        }
        public float EmInches => (NominalVerticalSize / 10f) / 72;
        public float EmXInches => (NominalHorizontalSize / 20f) / 72;

        public FND(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public FND(string desc, ushort pointSize) : base(Lookups.StructuredFieldID<FND>(), 0, 0, null)
        {
            Data = new byte[80];
            TypefaceDescription = desc;
            WeightClass = CommonMappings.eWeightClass.Medium;
            WidthClass = CommonMappings.eWidthClass.Medium;
            MaxVerticalSize = pointSize;
            NominalVerticalSize = pointSize;
            MinVerticalSize = pointSize;
        }

        public override void ParseData()
        {
            _nominalVerticalSize = (ushort)GetNumericValue(GetSectionedData(36, 2), false);
            _nominalHorizontalSize = (ushort)GetNumericValue(GetSectionedData(42, 2), false);
        }
    }
}