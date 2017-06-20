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
        public int Adjustment { get; private set; }
        public bool Forward { get; private set; }

        public SIA(byte[] data) : base(data) { }

        public override void ParseData()
        {
            Adjustment = (int)GetNumericValue(GetSectionedData(0, 2), true);
            Forward = Data[2] == 0x00;
        }
    }
}