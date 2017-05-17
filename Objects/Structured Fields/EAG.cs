using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class EAG : StructuredField
	{
		private static string _abbr = "EAG";
		private static string _title = "End Active Environment Group";
		private static string _desc = "Terminates the definition of an active environment group initiated by a BAG field.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.CHAR, "Name")
		};

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public EAG(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}