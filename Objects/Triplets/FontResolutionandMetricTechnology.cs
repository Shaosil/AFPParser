using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class FontResolutionAndMetricTechnology : Triplet
	{
		private static string _desc = "Specifies certain metric characteristics of a FOCA raster tech font character set which may have affected the document.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "Metric Technology")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x01, "Fixed Metrics" },
                    { 0x02, "Relative Metrics" }
                }
            },
            new Offset(1, Lookups.DataTypes.CODE, "Raster Pattern Resolution Base") { Mappings = CommonMappings.AxisBase },
            new Offset(2, Lookups.DataTypes.UBIN, "Raster Units per Base")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public FontResolutionAndMetricTechnology(byte[] allData) : base(allData) { }
	}
}