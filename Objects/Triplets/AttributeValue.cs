using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class AttributeValue : Triplet
	{
		private static string _desc = "Specifies a value for a document attribute";

		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>()
		{
			new Offset(2, Lookups.DataTypes.EMPTY, ""),
			new Offset(4, Lookups.DataTypes.CHAR, "Attribute Value")
		};

		public AttributeValue(byte[] allData) : base(allData) { }
	}
}