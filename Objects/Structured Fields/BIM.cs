using AFPParser.Containers;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    [ContainerType(typeof(IOCAImageContainer))]
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
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public string ObjectName { get; private set; }

		public BIM(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public override void ParseData()
        {
            base.ParseData();

            ObjectName = GetReadableDataPiece(0, 8);
        }
    }
}