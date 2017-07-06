using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class ERG : StructuredField
    {
        private static string _abbr = "ERG";
        private static string _title = "End Resource Group";
        private static string _desc = "Terminates the definition of a resource group initiated by a Begin Resource Group structured field.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Resource Group Name"),
            new Offset(8, Lookups.DataTypes.TRIPS, "")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public ERG(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }
    }
}