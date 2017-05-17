using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BOG : StructuredField
	{
		private static string _abbr = "BOG";
		private static string _title = "Begin Object Environment Group";
		private static string _desc = "The Begin Document structured field names and begins the documentThe Begin Object Environment Group structured field begins an Object Environment Group, which establishes the environment parameters for the object. The scope of an object environment group is its containing object..";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public BOG(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}