using System.Collections.Generic;
using System.Linq;

namespace AFPParser.StructuredFields
{
    public class FNM : StructuredField
    {
        private static string _abbr = "FNM";
        private static string _title = "Font Patterns Map";
        private static string _desc = "Describes some characteristics of the font character patterns.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "Character Box Width"),
            new Offset(2, Lookups.DataTypes.UBIN, "Character Box Height"),
            new Offset(4, Lookups.DataTypes.UBIN, "Pattern Data Offset")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => true;
        protected override int RepeatingGroupStart => 0;
        protected override int RepeatingGroupLength
        {
            get
            {
                int length = Data.Length;
                FNC control = LowestLevelContainer.GetStructure<FNC>();
                if (control != null)
                    length = control.Data[21];

                return length;
            }
        }
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public IReadOnlyList<PatternData> AllPatternData { get; private set; }

        // Repeating groups get parsed into this class
        public class PatternData
        {
            public ushort BoxWidth { get; set; }
            public ushort BoxHeight { get; set; }
            public uint DataOffset { get; set; }

            public PatternData(ushort width, ushort height, uint offset)
            {
                BoxWidth = width;
                BoxHeight = height;
                DataOffset = offset;
            }
        }

        public FNM(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public override void ParseData()
        {
            List<PatternData> allData = new List<PatternData>();

            // Load all patterns - repeating groups are always length 8
            for (int i = 0; i < Data.Length; i += 8)
            {
                byte[] widthBytes = GetSectionedData(i, 2);
                byte[] heightBytes = GetSectionedData(i + 2, 2);
                byte[] offsetBytes = GetSectionedData(i + 4, 4);

                ushort width = (ushort)GetNumericValue(widthBytes, false);
                ushort height = (ushort)GetNumericValue(heightBytes, false);
                uint offset = (uint)GetNumericValue(offsetBytes, false);

                allData.Add(new PatternData(width, height, offset));
            }

            // Set our readonly list
            AllPatternData = allData;
        }
    }
}