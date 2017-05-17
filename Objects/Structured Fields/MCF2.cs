using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MCF2 : StructuredField
	{
		private static string _abbr = "MCF";
		private static string _title = "Map Coded Font (Format 2)";
		private static string _desc = "The Map Coded Font structured field maps a unique coded font resource local ID, which may be embedded one or more times within an object's data and descriptor, to the identifier of a coded font resource object. This identifier may be specified in one of the following formats: A coded font Global Resource Identifier(GRID), A coded font name, A combination of code page name and font character set name";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public MCF2(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}