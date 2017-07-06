using System.Collections.Generic;

namespace AFPParser.Triplets
{
    public class MODCAInterchangeSet : Triplet
    {
        private static string _desc = "Identifies the interchange set and the data stream type.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CODE, "Interchange Set") { Mappings = new Dictionary<byte, string>() { { 0x01, "Presentation" } } },
            new Offset(1, Lookups.DataTypes.CODE, "Interchange Set Identifier")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x09, "MO:DCA IS/1" },
                    { 0x0C, "Retired Value" },
                    { 0x0D, "MO:DCA IS/3" }
                }
            }
        };

        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public MODCAInterchangeSet(byte id, byte[] data) : base(id, data) { }
    }
}