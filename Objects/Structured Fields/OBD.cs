using System.Linq;
using AFPParser.Triplets;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class OBD : StructuredField
    {
        private static string _abbr = "OBD";
        private static string _title = "Object Area Descriptor";
        private static string _desc = "The Object Area Descriptor structured field specifies the size and attributes of an object area presentation space.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.TRIPS, "")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public OBD(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
    }
}