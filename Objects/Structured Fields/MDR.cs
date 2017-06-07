using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MDR : StructuredField
	{
		private static string _abbr = "MDR";
		private static string _title = "Map Data Resource";
		private static string _desc = "The Map Data Resource structured field specifies resources that are required for presentation. Each resource reference is defined in a repeating group and is identified with a file name, the identifier of a begin structured field for the resource, or any other identifier associated with the resource.The MDR repeating group may additionally specify a local or internal identifier for the resource object. Such a local identifier may be embedded one or more times within an object's data.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public MDR(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
	}
}