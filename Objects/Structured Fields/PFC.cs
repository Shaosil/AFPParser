using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class PFC : StructuredField
	{
		private static string _abbr = "PFC";
		private static string _title = "Presentation Fidelity Control";
		private static string _desc = "The Presentation Fidelity Control structured field specifies the user fidelity requirements for data presented on physical media and for operations performed on physical media.The scope of the Presentation Fidelity Control structured field is the document or print file controlled by the form map that contains this structured field.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public PFC(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}