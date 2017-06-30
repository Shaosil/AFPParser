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

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

		public AttributeValue(string id, byte[] introcuder, byte[] data) : base(id, introcuder, data) { }
	}
}