using System.Collections.Generic;
using System.Linq;

namespace AFPParser.StructuredFields
{
    public class FNI : StructuredField
    {
        private static string _abbr = "FNI";
        private static string _title = "Font Index";
        private static string _desc = "Maps a descriptive font name to a font member name in a font library.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Graphic Character GID"),
            new Offset(8, Lookups.DataTypes.UBIN, "Character Increment"),
            new Offset(10, Lookups.DataTypes.SBIN, "Ascender Height"),
            new Offset(12, Lookups.DataTypes.SBIN, "Descender Depth"),
            new Offset(14, Lookups.DataTypes.EMPTY, ""),
            new Offset(16, Lookups.DataTypes.UBIN, "FNM Index"),
            new Offset(18, Lookups.DataTypes.SBIN, "A-Space"),
            new Offset(20, Lookups.DataTypes.SBIN, "B-Space"),
            new Offset(22, Lookups.DataTypes.SBIN, "C-Space"),
            new Offset(24, Lookups.DataTypes.EMPTY, ""),
            new Offset(26, Lookups.DataTypes.SBIN, "Baseline Offset"),
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
                    length = control.Data[15];

                return length;
            }
        }
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        private List<Info> _infoList = new List<Info>();
        public IReadOnlyList<Info> InfoList
        {
            get { return _infoList; }
            private set
            {
                _infoList = value.ToList();

                // Update the data stream
                Data = new byte[value.Count * 28];
                for (int i = 0; i < value.Count; i++)
                {
                    PutStringInData(value[i].GCGID, (i * 28) + 0, 8);
                    PutNumberInData(value[i].CharIncrement, (i * 28) + 8);
                    PutNumberInData(value[i].AscenderHeight, (i * 28) + 10);
                    PutNumberInData(value[i].DescenderDepth, (i * 28) + 12);
                    PutNumberInData(value[i].FNMIndex, (i * 28) + 16);
                    PutNumberInData(value[i].ASpace, (i * 28) + 18);
                    PutNumberInData(value[i].BSpace, (i * 28) + 20);
                    PutNumberInData(value[i].CSpace, (i * 28) + 22);
                    PutNumberInData(value[i].BaselineOffset, (i * 28) + 26);
                }
            }
        }

        public FNI(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public FNI(List<Info> infoList) : base(Lookups.StructuredFieldID<FNI>(), 0, 0, null)
        {
            InfoList = infoList;
        }

        public override void ParseData()
        {
            // Store properties of each character
            List<Info> allInfo = new List<Info>();
            int curIndex = 0;

            while (curIndex < Data.Length)
            {
                string gid = GetReadableDataPiece(curIndex + 0, 8);
                ushort charInc = GetNumericValueFromData<ushort>(curIndex + 8, 2);
                short ascHeight = GetNumericValueFromData<short>(curIndex + 10, 2);
                short descDepth = GetNumericValueFromData<short>(curIndex + 12, 2);
                ushort fnmIdx = GetNumericValueFromData<ushort>(curIndex + 16, 2);
                short a = GetNumericValueFromData<short>(curIndex + 18, 2);
                ushort b = GetNumericValueFromData<ushort>(curIndex + 20, 2);
                short c = GetNumericValueFromData<short>(curIndex + 22, 2);
                short baseline = GetNumericValueFromData<short>(curIndex + 26, 2);

                allInfo.Add(new Info(gid, charInc, ascHeight, descDepth, fnmIdx, a, b, c, baseline));

                curIndex += RepeatingGroupLength;
            }

            _infoList = allInfo;
        }

        public class Info
        {
            public string GCGID { get; private set; }
            public ushort CharIncrement { get; private set; }
            public short AscenderHeight { get; private set; }
            public short DescenderDepth { get; private set; }
            public ushort FNMIndex { get; private set; }
            public short ASpace { get; private set; }
            public ushort BSpace { get; private set; }
            public short CSpace { get; private set; }
            public short BaselineOffset { get; private set; }

            public Info(string gid, ushort charInc, short ascHeight, short descDepth, ushort fnmIdx, short a, ushort b, short c, short baseline)
            {
                GCGID = gid;
                CharIncrement = charInc;
                AscenderHeight = ascHeight;
                DescenderDepth = descDepth;
                FNMIndex = fnmIdx;
                ASpace = a;
                BSpace = b;
                CSpace = c;
                BaselineOffset = baseline;
            }
        }
    }
}