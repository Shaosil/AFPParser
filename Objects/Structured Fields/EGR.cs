using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EGR : StructuredField
	{
		private static string _abbr = "EGR";
		private static string _title = "End Graphics Object";
		private static string _desc = "The End Graphics Object structured field terminates the current graphics object initiated by a Begin Graphics Object structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public EGR(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}