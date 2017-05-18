using System.Collections.Generic;

namespace AFPParser.Triplets
{
	public class AttributeValue : Triplet
	{
		private static string _desc = "Specifies a value for a document attribute";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, ""),
            new Offset(2, Lookups.DataTypes.CHAR, "Attribute Value")
        };

        protected override string Description => _desc;
        protected override List<Offset> Offsets => _oSets;

		public AttributeValue(byte[] allData) : base(allData) { }
	}
}