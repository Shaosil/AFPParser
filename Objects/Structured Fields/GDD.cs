using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class GDD : StructuredField
	{
		private static string _abbr = "GDD";
		private static string _title = "Graphics Data Descriptor";
		private static string _desc = "The Graphics Data Descriptor structured field contains the descriptor data for a graphics object.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public GDD(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
	}
}