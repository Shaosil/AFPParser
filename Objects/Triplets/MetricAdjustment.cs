using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class MetricAdjustment : Triplet
    {
        private static string _desc = "Supplies metric values that can be used to adjust some of the metrics in an outline coded font.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "Unit Base") { Mappings = Lookups.CommonMappings.AxisBase },
            new Offset(1, Lookups.DataTypes.UBIN, "Units per X Base"),
            new Offset(3, Lookups.DataTypes.UBIN, "Units per Y Base"),
            new Offset(5, Lookups.DataTypes.SBIN, "Uniform Char Horizontal Increment"),
            new Offset(7, Lookups.DataTypes.SBIN, "Uniform Char Vertical Increment"),
            new Offset(9, Lookups.DataTypes.SBIN, "Baseline Horizontal Offset"),
            new Offset(11, Lookups.DataTypes.SBIN, "Baseline Vertical Offset"),
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public MetricAdjustment(byte[] allData) : base(allData) { }
	}
}