using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class PPO : StructuredField
	{
		private static string _abbr = "PPO";
		private static string _title = "Preprocess Presentation Object";
		private static string _desc = "The Preprocess Presentation Object structured field specifies presentation parameters for a data object that has been mapped as a resource.These parameters allow the presentation device to preprocess and cache the object so that it is in presentation-ready format when it is included with a subsequent include structured field in the document. Such preprocessing may involve a rasterization or RIP of the object, but is not limited to that. The resource is identified with a file name, the identifier of a begin structured field for the resource, or any other identifier associated with the resource. The referenced resource and all required secondary resources must previously have been mapped with an MDR or an MPO in the same environment group.  Preprocessing is not supported for objects that are included with structures that are outside the document. Examples of such objects are medium overlays and PMC overlays, both of which are included with structures in the form map.";
        private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public PPO(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
	}
}