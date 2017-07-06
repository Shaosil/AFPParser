using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class LND : StructuredField
	{
		private static string _abbr = "LND";
		private static string _title = "Line Descriptor";
		private static string _desc = "The Line Descriptor structured field contains information, such as line position, text orientation, font selection, field selection, and conditional processing identification, used to format line data. Note: The LNDs in a Data Map are numbered sequentially, starting with 1. Values from 1 through the number of LNDs are allowed.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public LND(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}