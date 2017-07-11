using System.Collections.Generic;
using System.Diagnostics;
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

        // Parsed Data
        private List<PatternData> _allPatternData = new List<PatternData>();
        public IReadOnlyList<PatternData> AllPatternData
        {
            get { return _allPatternData; }
            private set
            {
                _allPatternData = value.ToList();

                // Update data stream
                Data = new byte[_allPatternData.Count * 8];
                for (int i = 0; i < value.Count; i++)
                {
                    PutNumberInData(value[i].BoxWidth, (i * 8));
                    PutNumberInData(value[i].BoxHeight, (i * 8) + 2);
                    PutNumberInData(value[i].DataOffset, (i * 8) + 4);
                }
            }
        }

        // Repeating groups get parsed into this class
        [DebuggerDisplay("{BoxWidth}/{BoxHeight}/{DataOffset}")]
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

        public FNM(List<PatternData> patData) : base(Lookups.StructuredFieldID<FNM>(), 0, 0, null)
        {
            AllPatternData = patData;
        }

        public override void ParseData()
        {
            List<PatternData> allData = new List<PatternData>();

            // Load all patterns - repeating groups are always length 8
            for (int i = 0; i < Data.Length; i += 8)
            {
                byte[] widthBytes = GetSectionedData(i, 2);
                byte[] heightBytes = GetSectionedData(i + 2, 2);
                byte[] offsetBytes = GetSectionedData(i + 4, 4);

                ushort width = GetNumericValue<ushort>(widthBytes);
                ushort height = GetNumericValue<ushort>(heightBytes);
                uint offset = GetNumericValue<uint>(offsetBytes);

                allData.Add(new PatternData(width, height, offset));
            }

            // Set our readonly list
            _allPatternData = allData;
        }
    }
}