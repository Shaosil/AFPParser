using System.Collections.Generic;

namespace AFPParser.PTXControlSequences
{
    public class SIA : PTXControlSequence
    {
        private static string _abbr = "SIA";
        private static string _desc = "Set Intercharacter Adjustment";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.SBIN, "Adjustment"),
            new Offset(2, Lookups.DataTypes.CODE, "Direction")
            {
                Mappings = new Dictionary<byte, string>() { { 0x00, "Increment" }, { 0x01, "Decrement" } }
            }
        };

        public override string Abbreviation => _abbr;
        public override string Description => _desc;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public short Adjustment { get; private set; }
        public bool Forward { get; private set; }

        public SIA(byte id, byte[] sequence, byte[] data) : base(id, sequence, data) { }

        public override void ParseData()
        {
            Adjustment = GetNumericValueFromData<short>(0, 2);
            Forward = Data[2] == 0x00;
        }
    }
}