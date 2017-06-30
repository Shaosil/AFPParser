using System.Text;
using System.Drawing;
using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
	public class STC : PTXControlSequence
	{
		private static string _abbr = "STC";
		private static string _desc = "Set Text Color";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, "Color - CUSTOM PARSED")
        };

        public override string Abbreviation => _abbr;
		public override string Description => _desc;
		public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public Color TextColor { get; private set; }

		public STC(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }

        public override void ParseData()
        {
            if (Lookups.StandardOCAColors.ContainsKey(Data[1]))
                TextColor = Lookups.StandardOCAColors[Data[1]];
            else
                TextColor = Color.Black;
        }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            // Only one offset
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(TextColor.ToString());
            return sb.ToString();
        }
    }
}
