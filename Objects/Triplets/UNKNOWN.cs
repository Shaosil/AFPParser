using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class UNKNOWN : Triplet
    {
        public override string Description { get { return $"UNKNOWN TRIPLET: {HexID}"; } }

        public override IReadOnlyList<Offset> Offsets { get { return new List<Offset>(); } }

        public UNKNOWN(string id, byte[] introcuder, byte[] data) : base(id, introcuder, data) { }
    }
}