using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class MCF1 : StructuredField
	{
		private static string _abbr = "MCF";
		private static string _title = "Map Coded Font (Format 1)";
		private static string _desc = "Identifies the correspondence between external font names and resource local identifiers.";
		private static List<Offset> _oSets = new List<Offset>()
		{
			new Offset(0, Lookups.DataTypes.UBIN, "Coded Font ID"),
			new Offset(1, Lookups.DataTypes.EMPTY, ""),
			new Offset(2, Lookups.DataTypes.CODE, "Font section ID - CUSTOM PARSED"),
			new Offset(3, Lookups.DataTypes.EMPTY, ""),
			new Offset(4, Lookups.DataTypes.CHAR, "Coded Font Name"),
			new Offset(12, Lookups.DataTypes.CHAR, "Code Page Name"),
			new Offset(20, Lookups.DataTypes.CHAR, "Font Character Set Name"),
			new Offset(28, Lookups.DataTypes.CODE, "Character Rotation") { Mappings = CommonMappings.Rotations }
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => true;
		protected override int RepeatingGroupStart => 4;
        protected override int RepeatingGroupLength => Data[0];
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public IReadOnlyList<MCF1Data> MappedData { get; private set; }

        public MCF1(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }

        public override void ParseData()
        {
            // Loop through each repeating group and parse out pieces of data that matter
            List<MCF1Data> allFontData = new List<MCF1Data>();
            int curIndex = 4;
            int skip = Data[0];

            while (curIndex < Data.Length)
            {
                int id = (int)GetNumericValue(GetSectionedData(curIndex, 1), false);
                string cfName = GetReadableDataPiece(curIndex + 4, 8);
                string cpName = GetReadableDataPiece(curIndex + 12, 8);
                string fcsName = GetReadableDataPiece(curIndex + 20, 8);
                allFontData.Add(new MCF1Data(id, cfName, cpName, fcsName));

                curIndex += skip;
            }

            MappedData = allFontData;
        }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            StringBuilder sb = new StringBuilder();

            switch (oSet.StartingIndex)
            {
                case 2: // Font section index
                    // Single bytes are 0x00
                    if (sectionedData[0] == 0x00)
                        sb.AppendLine("Font Section ID: Single byte coded font");
                    // Double bytes are basically everything else
                    else if (sectionedData[0] >= 0x41 && sectionedData[0] <= 0xFE)
                        sb.AppendLine("Font Section ID: Double byte coded font");
                    else
                        sb.AppendLine($"Font Section ID: Unknown coded font byte size (0x{sectionedData[0].ToString("X")})");

                    break;
 
                default:
                    return base.GetSingleOffsetDescription(oSet, sectionedData);
            }

            return sb.ToString();

        }

        public class MCF1Data
        {
            public int ID { get; private set; }
            public string CodedFontName { get; private set; }
            public string CodePageName { get; private set; }
            public string FontCharacterSetName { get; private set; }
            
            public MCF1Data(int id, string cfName, string cpName, string fcsName)
            {
                ID = id;
                CodedFontName = cfName;
                CodePageName = cpName;
                FontCharacterSetName = fcsName;
            }
        }
    }
}