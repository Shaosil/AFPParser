using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class BMM : StructuredField
	{
		private static string _abbr = "BMM";
		private static string _title = "Begin Medium Map";
		private static string _desc = "The Begin Medium Map structured field begins a medium map resource object. A medium map is a print control resource object that contains a complete set of controls for presenting pages on physical media such as sheets and for generating multiple copies of sheets with selectable modifications.These controls may be grouped into two categories: Medium level controls, Page level controls Medium level controls are controls that affect the medium, such as the specification of medium overlays, medium size, medium orientation, medium copies, simplex or duplex, medium finishing, media type, and media source and destination selection. These controls are defined by the Map Medium Overlay (MMO), Medium Descriptor(MDD), Medium Copy Count(MCC), Medium Finishing Control (MFC), Map Media Type(MMT), Map Media Destination(MMD), Presentation Environment Control(PEC), and Medium Modification Control(MMC) structured fields.Page level controls are controls that affect the pages that are placed on the medium, such as the specification of page modifications, page position, and page orientation.These controls are defined by the Map Page Overlay (MPO), Page Position(PGP), and Page Modification Control(PMC) structured fields.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public BMM(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}