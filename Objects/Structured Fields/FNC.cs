using System;
using System.Collections.Generic;
using System.Text;

namespace AFPParser.StructuredFields
{
    public class FNC : StructuredField
    {
        private static string _abbr = "FNC";
        private static string _title = "Font Control";
        private static string _desc = "Provides defaults and information about the font character set.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, ""),
            new Offset(1, Lookups.DataTypes.CODE, "Pattern Technology")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x05, ePatternTech.LaserMatrixNBitWide.ToString() },
                    { 0x1E, ePatternTech.CIDKeyedFont.ToString() },
                    { 0x1F, ePatternTech.PFBType1.ToString() }
                }
            },
            new Offset(2, Lookups.DataTypes.EMPTY, ""),
            new Offset(3, Lookups.DataTypes.BITS, "Font Use Flags")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Normal Printing|MICR Printing" },
                    { 0x01, "Complete Font|Extension Font" },
                    { 0x06, "Varied Raster Pattern Size|Fixed Raster Pattern Size" }
                }
            },
            new Offset(4, Lookups.DataTypes.CODE, "X Unit Base")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Fixed Metrics, 10 inches" },
                    { 0x02, "Relative Metrics" }
                }
            },
            new Offset(5, Lookups.DataTypes.CODE, "Y Unit Base")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "Fixed Metrics, 10 inches" },
                    { 0x02, "Relative Metrics" }
                }
            },
            new Offset(6, Lookups.DataTypes.UBIN, "X Units - CUSTOM PARSED"),
            new Offset(8, Lookups.DataTypes.UBIN, "Y Units - CUSTOM PARSED"),
            new Offset(10, Lookups.DataTypes.UBIN, "Max Character Box Width"),
            new Offset(12, Lookups.DataTypes.UBIN, "Max Character Box Height"),
            new Offset(14, Lookups.DataTypes.UBIN, "FNO Repeating Group Length"),
            new Offset(15, Lookups.DataTypes.UBIN, "FNI Repeating Group Length"),
            new Offset(16, Lookups.DataTypes.CODE, "Pattern Data Alignment Code")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "1 Byte Alignment" },
                    { 0x02, "4 Byte Alignment" },
                    { 0x03, "8 Byte Alignment" }
                }
            },
            new Offset(17, Lookups.DataTypes.UBIN, "Raster Pattern Data Count"),
            new Offset(20, Lookups.DataTypes.UBIN, "FNP Repeating Group Length"),
            new Offset(21, Lookups.DataTypes.UBIN, "FNM Repeating Group Length"),
            new Offset(22, Lookups.DataTypes.CODE, "Shape Resolution X Unit Base") { Mappings = CommonMappings.UnitBase },
            new Offset(23, Lookups.DataTypes.CODE, "Shape Resolution Y Unit Base") { Mappings = CommonMappings.UnitBase },
            new Offset(24, Lookups.DataTypes.UBIN, "Shape Units Per X Base - CUSTOM PARSED"),
            new Offset(26, Lookups.DataTypes.UBIN, "Shape Units Per Y Base - CUSTOM PARSED"),
            new Offset(28, Lookups.DataTypes.UBIN, "Outline Pattern Data Count"),
            new Offset(32, Lookups.DataTypes.EMPTY, ""),
            new Offset(35, Lookups.DataTypes.UBIN, "FNN Repeating Group Length"),
            new Offset(36, Lookups.DataTypes.UBIN, "FNN Data Count"),
            new Offset(40, Lookups.DataTypes.UBIN, "FNN Name Count"),
            new Offset(42, Lookups.DataTypes.TRIPS, "")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public enum ePatternTech { LaserMatrixNBitWide = 5, CIDKeyedFont = 30, PFBType1 = 31 }
        [Flags]
        public enum eFontUseFlags { MICRPrinting = 128, ExtendedFont = 64, FixedRasterSize = 2 }
        public enum eUnitBase { FixedMetrics10Inches = 0, RelativeMetrics = 2 }
        public enum eUnits { TwoFortyPPI = 2400, ThreeHundredPPI = 3000, OneThousandUnitsPerEm = 1000 }
        public enum ePatternAlignment { OneByte = 0, FourByte = 2, EightByte = 3 }

        private eUnits _xUnits;
        private eUnits _yUnits;
        private ushort _maxCharBoxWidth;
        private ushort _maxCharBoxHeight;
        private int _rasterPatternDataCount;
        private eUnits _xShapeResolution;
        private eUnits _yShapeResolution;

        public ePatternTech PatternTech
        {
            get { return (ePatternTech)Data[1]; }
            private set { Data[1] = (byte)value; }
        }
        public eFontUseFlags FontUseFlags
        {
            get { return (eFontUseFlags)Data[3]; }
            private set { Data[3] = (byte)value; }
        }
        public eUnitBase XUnitBase
        {
            get { return (eUnitBase)Data[4]; }
            private set { Data[4] = (byte)value; }
        }
        public eUnitBase YUnitBase
        {
            get { return (eUnitBase)Data[5]; }
            private set { Data[5] = (byte)value; }
        }
        public eUnits XUnits
        {
            get { return _xUnits; }
            private set
            {
                _xUnits = value;
                PutNumberInData((ushort)value, 6);
            }
        }
        public eUnits YUnits
        {
            get { return _yUnits; }
            private set
            {
                _yUnits = value;
                PutNumberInData((ushort)value, 8);
            }
        }
        public ushort MaxBoxWidth
        {
            get { return _maxCharBoxWidth; }
            private set
            {
                _maxCharBoxWidth = value;
                PutNumberInData(value, 10);
            }
        }
        public ushort MaxBoxHeight
        {
            get { return _maxCharBoxHeight; }
            private set
            {
                _maxCharBoxHeight = value;
                PutNumberInData(value, 12);
            }
        }
        public byte FNORGLength
        {
            get { return Data[14]; }
            private set { Data[14] = value; }
        }
        public byte FNIRGLength
        {
            get { return Data[15]; }
            private set { Data[15] = value; }
        }
        public ePatternAlignment PatternAlignment
        {
            get { return (ePatternAlignment)Data[16]; }
            private set { Data[16] = (byte)value; }
        }
        public int RasterDataCount
        {
            get { return _rasterPatternDataCount; }
            private set
            {
                _rasterPatternDataCount = value;
                PutNumberInData(value, 17, 3);
            }
        }
        public byte FNPRGLength
        {
            get { return Data[20]; }
            private set { Data[20] = value; }
        }
        public byte FNMRGLength
        {
            get { return Data[21]; }
            private set { Data[21] = value; }
        }
        public eUnits XShapeResolution
        {
            get { return _xShapeResolution; }
            private set
            {
                _xShapeResolution = value;
                PutNumberInData((ushort)value, 24);
            }
        }
        public eUnits YShapeResolution
        {
            get { return _yShapeResolution; }
            private set
            {
                _yShapeResolution = value;
                PutNumberInData((ushort)value, 26);
            }
        }

        public FNC(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public FNC(ushort maxBitWidth, ushort maxBitHeight, int rasterByteCount) : base(Lookups.StructuredFieldID<FNC>(), 0, 0, null)
        {
            Data = new byte[28];
            PatternTech = ePatternTech.LaserMatrixNBitWide;
            XUnitBase = eUnitBase.RelativeMetrics;
            YUnitBase = eUnitBase.RelativeMetrics;
            XUnits = eUnits.OneThousandUnitsPerEm;
            YUnits = eUnits.OneThousandUnitsPerEm;
            MaxBoxWidth = maxBitWidth;
            MaxBoxHeight = maxBitHeight;
            FNORGLength = 26; // Constant
            FNIRGLength = 28; // Constant for raster
            PatternAlignment = ePatternAlignment.OneByte;
            RasterDataCount = rasterByteCount;
            FNPRGLength = 22; // Constant
            FNMRGLength = 8; // Constant for raster
            XShapeResolution = eUnits.ThreeHundredPPI;
            YShapeResolution = eUnits.ThreeHundredPPI;
        }

        public override void ParseData()
        {
            base.ParseData();
            
            _maxCharBoxWidth = GetNumericValueFromData<ushort>(10, 2);
            _maxCharBoxHeight = GetNumericValueFromData<ushort>(12, 2);
            _rasterPatternDataCount = GetNumericValueFromData<ushort>(17, 2);
        }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            switch (oSet.StartingIndex)
            {
                case 6:
                case 8:
                case 24:
                case 26:
                    StringBuilder sb = new StringBuilder();
                    ushort units = GetNumericValueFromData<ushort>(oSet.StartingIndex, 2);
                    if (units != 1000) units /= 10;

                    if (oSet.StartingIndex == 24 || oSet.StartingIndex == 26) sb.Append("Shape ");
                    if (oSet.StartingIndex == 6 || oSet.StartingIndex == 24) sb.Append("X");
                    else sb.Append("Y");
                    sb.Append($" Units per ");
                    if (units == 1000) sb.Append("Em: ");
                    else sb.Append("Base: ");
                    sb.AppendLine($"{units}");

                    return sb.ToString();

                default:
                    return base.GetSingleOffsetDescription(oSet, sectionedData);
            }
        }
    }
}