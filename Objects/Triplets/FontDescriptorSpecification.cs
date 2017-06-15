using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class FontDescriptorSpecification : Triplet
	{
		private static string _desc = "Specifies the attributes of the desired font in a coded font reference.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "Char Stroke Thickness") { Mappings = Lookups.CommonMappings.WeightClass },
            new Offset(1, Lookups.DataTypes.CHAR, "Char Width Class") { Mappings = Lookups.CommonMappings.WidthClass },
            new Offset(2, Lookups.DataTypes.UBIN, "Vertical Font Size"),
            new Offset(4, Lookups.DataTypes.UBIN, "Horizontal Font Size"),
            new Offset(6, Lookups.DataTypes.BITS, "Font Design Flags") { Mappings = Lookups.CommonMappings.FontDesignFlags },
            new Offset(7, Lookups.DataTypes.EMPTY, ""),
            new Offset(17, Lookups.DataTypes.BITS, "Font Use Flags")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x01, "Bitmapped Font|Outline or Vector Font" },
                    { 0x02, "Not Transformed|Transformable" }
                }
            }
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public FontDescriptorSpecification(byte[] allData) : base(allData) { }
	}
}