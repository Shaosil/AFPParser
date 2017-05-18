using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class ColorSpecification : Triplet
	{
		private static string _desc = "Specifies a color value and defines the color space and encoding for that value.";

		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>()
		{
			new Offset(2, Lookups.DataTypes.EMPTY, ""),
			new Offset(3, Lookups.DataTypes.CODE, "Color Space")
			{
				Mappings = new Dictionary<byte, string>()
				{
					{ 0x01, "RGB" },
					{ 0x04, "CMYK" },
					{ 0x06, "Highlight color space" },
					{ 0x08, "CIELAB" },
					{ 0x40, "Standard OCA color space" }
				}
			},
			new Offset(4, Lookups.DataTypes.EMPTY, ""),
			new Offset(8, Lookups.DataTypes.UBIN, "Component 1 bits"),
			new Offset(9, Lookups.DataTypes.UBIN, "Component 2 bits"),
			new Offset(10, Lookups.DataTypes.UBIN, "Component 3 bits"),
			new Offset(11, Lookups.DataTypes.UBIN, "Component 4 bits"),
			new Offset(12, Lookups.DataTypes.COLOR, "")
		};

		public ColorSpecification(byte[] allData) : base(allData) { }
	}
}