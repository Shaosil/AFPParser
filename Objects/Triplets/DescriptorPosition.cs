using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class DescriptorPosition : Triplet
    {
        private static string _desc = "Used to associate an object area position field with an object area description field.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "Object Area Position ID")
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public int OBPID { get; private set; }

        public DescriptorPosition(string id, byte[] introcuder, byte[] data) : base(id, introcuder, data) { }

        public override void ParseData()
        {
            OBPID = Data[0];
        }
    }
}