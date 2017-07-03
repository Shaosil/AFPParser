using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MCF2 : StructuredField
	{
		private static string _abbr = "MCF";
		private static string _title = "Map Coded Font (Format 2)";
		private static string _desc = "The Map Coded Font structured field maps a unique coded font resource local ID, which may be embedded one or more times within an object's data and descriptor, to the identifier of a coded font resource object. This identifier may be specified in one of the following formats: A coded font Global Resource Identifier(GRID), A coded font name, A combination of code page name and font character set name";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, ""), // RGLength
            new Offset(2, Lookups.DataTypes.TRIPS, "")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => true;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public MCF2(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}