using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MCC : StructuredField
	{
		private static string _abbr = "MCC";
		private static string _title = "Medium Copy Count";
		private static string _desc = "The Medium Copy Count structured field specifies the number of copies of each medium, or sheet, to be presented, and the modifications that apply to each copy. This specification is called a copy group.The MCC contains repeating groups that specify copy subgroups, such that each copy subgroup may be specified independently of any other copy subgroup. For each copy subgroup, the number of copies, as well as the modifications to be applied to each copy, is specified by the repeating group.If the modifications for a copy subgroup specify duplexing, that copy subgroup and all successive copy subgroups are paired such that the first copy subgroup in the pair specifies the copy count as well as themodifications to be applied to the front side of each copy, and the second copy subgroup in the pair specifies the same copy count as well as an independent set of modifications to be applied to the back side of each copy.The pairing of copy subgroups continues as long as duplexing is specified.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public MCC(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}