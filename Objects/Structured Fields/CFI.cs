using System.Text;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class CFI : StructuredField
	{
		private static string _abbr = "CFI";
		private static string _title = "Coded Font Index";
		private static string _desc = "Names the font character sets and code pages for the font.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Font Character Set Name"),
            new Offset(8, Lookups.DataTypes.CHAR, "Code Page Name"),
            new Offset(16, Lookups.DataTypes.UBIN, "Vertical Font Size (1440ths of an inch)"),
            new Offset(18, Lookups.DataTypes.UBIN, "Horizontal Scale Factor (1440ths of an inch)"),
            new Offset(20, Lookups.DataTypes.EMPTY, ""),
            new Offset(24, Lookups.DataTypes.UBIN, "Section - CUSTOM PARSED")
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
                return LowestLevelContainer.GetStructure<CFC>().CFIRGLength;
            }
        }
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public IReadOnlyList<FontInfo> FontInfoList { get; private set; }

		public CFI(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }

        public override void ParseData()
        {
            // Gather all font info
            List<FontInfo> infoList = new List<FontInfo>();
            int curIndex = 0;
            while (curIndex < Data.Length)
            {
                infoList.Add(new FontInfo(GetReadableDataPiece(0, 8), GetReadableDataPiece(8, 8)));

                curIndex += RepeatingGroupLength;
            }

            FontInfoList = infoList;
        }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            if (oSet.StartingIndex != 24)
                return base.GetSingleOffsetDescription(oSet, sectionedData);

            StringBuilder sb = new StringBuilder("Section: ");
            if (sectionedData[0] == 0)
                sb.AppendLine("Single byte");
            else
                sb.AppendLine("Double byte");

            return sb.ToString();
        }

        public class FontInfo
        {
            public string FontCharacterSetName { get; private set; }
            public string CodePageName { get; private set; }

            public FontInfo(string fcsName, string cpName)
            {
                FontCharacterSetName = fcsName;
                CodePageName = cpName;
            }
        }
    }
}