using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class TRN : PTXControlSequence
	{
		private static string _abbr = "TRN";
		private static string _desc = "Transparent Data";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
        protected override List<Offset> Offsets => new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Data")
        };

		public TRN(byte[] data) : base(data) { }
	}
}
