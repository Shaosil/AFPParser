using System.Collections.Generic;

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
        public IReadOnlyList<Info> InfoList { get; private set; }

		public FNI(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }

        public override void ParseData()
        {
            // Store properties of each character
            List<Info> allInfo = new List<Info>();
            int curIndex = 0;

            while (curIndex < Data.Length)
            {
                string gid = GetReadableDataPiece(curIndex + 0, 8);
                int charInc = (int)GetNumericValue(GetSectionedData(curIndex + 8, 2), false);
                int ascHeight = (int)GetNumericValue(GetSectionedData(curIndex + 10, 2), true);
                int descDepth = (int)GetNumericValue(GetSectionedData(curIndex + 12, 2), true);
                int fnmIdx = (int)GetNumericValue(GetSectionedData(curIndex + 16, 2), false);
                int a = (int)GetNumericValue(GetSectionedData(curIndex + 18, 2), true);
                int b = (int)GetNumericValue(GetSectionedData(curIndex + 20, 2), true);
                int c = (int)GetNumericValue(GetSectionedData(curIndex + 22, 2), true);
                int baseline = (int)GetNumericValue(GetSectionedData(curIndex + 26, 2), true);

                allInfo.Add(new Info(gid, charInc, ascHeight, descDepth, fnmIdx, a, b, c, baseline));

                curIndex += RepeatingGroupLength;
            }

            InfoList = allInfo;
        }
        public class Info
        {
            public string GCGID { get; private set; }
            public int CharIncrement { get; private set; }
            public int AscenderHeight { get; private set; }
            public int DescenderDepth { get; private set; }
            public int FNMIndex { get; private set; }
            public int ASpace { get; private set; }
            public int BSpace { get; private set; }
            public int CSpace { get; private set; }
            public int BaselineOffset { get; private set; }

            public Info(string gid, int charInc, int ascHeight, int descDepth, int fnmIdx, int a, int b, int c, int baseline)
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