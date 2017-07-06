using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BMO : StructuredField
	{
		private static string _abbr = "BMO";
		private static string _title = "Begin Overlay";
		private static string _desc = "The Begin Overlay structured field begins an overlay. An overlay contains an active environment group to establish parameters such as the size of the overlay's presentation space and the fonts to be used by the data objects.It may also contain any mixture of: Bar code objects, Graphics objects, Image objects, Object containers, Presentation text objects";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public BMO(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}