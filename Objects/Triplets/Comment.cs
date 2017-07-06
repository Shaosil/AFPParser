using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class Comment : Triplet
    {
        private static string _desc = "Used to include comments for documentation purposes.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Comment")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public Comment(byte id, byte[] data) : base(id, data) { }
    }
}