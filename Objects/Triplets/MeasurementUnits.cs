using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class MeasurementUnits : Triplet
    {
        private static string _desc = "Specifies units of measure for a presentation space.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "X Unit Base") { Mappings = CommonMappings.UnitBase },
            new Offset(1, Lookups.DataTypes.CODE, "Y Unit Base") { Mappings = CommonMappings.UnitBase },
            new Offset(2, Lookups.DataTypes.UBIN, "X Units per Base"),
            new Offset(4, Lookups.DataTypes.UBIN, "Y Units per Base")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public Converters.eMeasurement BaseUnit { get; private set; }
        public ushort XUnitsPerBase { get; private set; }
        public ushort YUnitsPerBase { get; private set; }

        public MeasurementUnits(byte id, byte[] data) : base(id, data) { }

        public override void ParseData()
        {
            BaseUnit = (Converters.eMeasurement)Data[0];
            XUnitsPerBase = GetNumericValueFromData<ushort>(2, 2);
            YUnitsPerBase = GetNumericValueFromData<ushort>(4, 2);
        }
    }
}