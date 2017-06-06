using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class SIA : PTXControlSequence
	{
		private static string _abbr = "SIA";
		private static string _desc = "Set Intercharacter Adjustment";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.SBIN, "Adjustment"),
            new Offset(2, Lookups.DataTypes.CODE, "Direction")
            {
                Mappings = new Dictionary<byte, string>() { { 0x00, "Increment" }, { 0x01, "Decrement" } }
            }
        };

        public override string Abbreviation => _abbr;
		protected override string Description => _desc;
		protected override List<Offset> Offsets => _oSets;

		public SIA(byte[] data) : base(data) { }
	}
}