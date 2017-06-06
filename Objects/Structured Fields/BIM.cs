using AFPParser.Containers;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    [ContainerType(typeof(ImageObjectContainer))]
    public class BIM : StructuredField
    {
        private static string _abbr = "BIM";
        private static string _title = "Begin Image Object IO";
        private static string _desc = "The Begin Image Object structured field begins an IOCA image data object, which becomes the current data object.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Image Data Object Name"),
            new Offset(8, Lookups.DataTypes.TRIPS, "")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        protected override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        protected override List<Offset> Offsets => _oSets;

		public BIM(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }
    }
}