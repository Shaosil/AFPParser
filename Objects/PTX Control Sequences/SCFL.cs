using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class SCFL : PTXControlSequence
	{
		private static string _abbr = "SCFL";
		private static string _desc = "Set Coded Font Local";

		public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, "Identifier")
        };

        public SCFL(byte[] data) : base(data) { }
	}
}
