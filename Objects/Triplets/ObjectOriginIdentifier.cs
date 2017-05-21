using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class ObjectOriginIdentifier : Triplet
    {
        private static string _desc = "";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, ""),
            new Offset(1, Lookups.DataTypes.CHAR, "Host ID"),
            new Offset(9, Lookups.DataTypes.CHAR, "Media ID"),
            new Offset(15, Lookups.DataTypes.CHAR, "Data Set ID")
        };

        protected override string Description => _desc;
        protected override List<Offset> Offsets => _oSets;

        public ObjectOriginIdentifier(byte[] allData) : base(allData) { }
    }
}