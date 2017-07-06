using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class IDM : StructuredField
	{
		private static string _abbr = "IDM";
		private static string _title = "Invoke Data Map";
		private static string _desc = "The Invoke Data Map structured field selects a new Data Map for printing line data and ends the current line-format page. With LND Data Maps, processing begins with the first Line Descriptor (LND) structured field of the invoked Data Map for the next line-format page. With RCD Data Maps, processing begins with the first Record Descriptor (RCD) structured field that matches the Record ID of the current line-data record. With XMD Data Maps, processing begins with the first XML Descriptor (XMD) structured field that matches the current Qualified Tag.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public IDM(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}