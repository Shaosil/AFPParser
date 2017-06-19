using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class MeasurementUnits : Triplet
    {
        private static string _desc = "Specifies units of measure for a presentation space.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "X Unit Base") { Mappings = CommonMappings.AxisBase },
            new Offset(1, Lookups.DataTypes.CODE, "Y Unit Base") { Mappings = CommonMappings.AxisBase },
            new Offset(2, Lookups.DataTypes.UBIN, "X Units per Base"),
            new Offset(4, Lookups.DataTypes.UBIN, "Y Units per Base")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public Converters.eMeasurement BaseUnit { get; private set; }
        public int XUnitsPerBase { get; private set; }
        public int YUnitsPerBase { get; private set; }

        public MeasurementUnits(byte[] allData) : base(allData) { }

        public override void ParseData()
        {
            BaseUnit = Converters.GetBaseUnit(Data[0]);
            XUnitsPerBase = (int)GetNumericValue(GetSectionedData(2, 2), false);
            YUnitsPerBase = (int)GetNumericValue(GetSectionedData(4, 2), false);
        }
    }
}