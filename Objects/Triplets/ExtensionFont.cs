using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class ExtensionFont : Triplet
    {
        private static string _desc = "Contains the GCSGID for the base font associated with an extension font.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "Graphic Character Set GID")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public ExtensionFont(byte id, byte[] introducer, byte[] data) : base(id, introducer, data) { }
    }
}