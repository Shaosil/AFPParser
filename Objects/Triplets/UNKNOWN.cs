using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class UNKNOWN : Triplet
    {
        protected override string Description { get { return $"UNKNOWN TRIPLET: {ID}"; } }

        protected override List<Offset> Offsets { get { return new List<Offset>(); } }

        public UNKNOWN(byte[] allData) : base (allData)
        { }
    }
}