using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class FDX : StructuredField
	{
		private static string _abbr = "FDX";
		private static string _title = "Fixed Data Text";
		private static string _desc = "The Fixed Data Text structured field contains text that can be selected and presented with LND, RCD or XMD structured fields in the Page Definition. This text is used when flag bit 7 of the LND, RCD or XMD is set to B'1'. Any number of FDX structured fields can appear, but the total number of data bytes must match bytes 0–1 of the Fixed Data Size (FDS) structured field. The output should fit on the page, and the fit can be affected by the size of the font used";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public FDX(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}