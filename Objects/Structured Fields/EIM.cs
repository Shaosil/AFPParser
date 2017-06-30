using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EIM : StructuredField
	{
		private static string _abbr = "EIM";
		private static string _title = "End Image Object IO";
		private static string _desc = "The End Image Object structured field terminates the current image object initiated by a Begin Image Object structured field.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Image Data Object Name"),
            new Offset(8, Lookups.DataTypes.TRIPS, "")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public EIM(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}