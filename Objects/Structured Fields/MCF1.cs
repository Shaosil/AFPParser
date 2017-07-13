using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private List<MCF1Data> _mappedData = new List<MCF1Data>();
        public IReadOnlyList<MCF1Data> MappedData
        {
            get { return _mappedData; }
            private set
            {
                _mappedData = value.ToList();

                // Update the data stream
                byte len = Data[0];
                Data = new byte[4 + (value.Count * len)];
                Data[0] = (byte)(value.Any(v => v.Rotation != null) ? 30 : 28);
                for (int i = 0; i < value.Count; i++)
                {
                    Data[4 + (i * len)] = value[i].ID;
                    Data[4 + (i * len) + 2] = (byte)value[i].FontSectionID;
                    PutStringInData(value[i].CodedFontName, 4 + (i * len) + 4, 8);
                    PutStringInData(value[i].CodePageName, 4 + (i * len) + 12, 8);
                    PutStringInData(value[i].FontCharacterSetName, 4 + (i * len) + 20, 8);
                    if (value[i].Rotation != null && len == 30)
                        Data[4 + (i * len) + 28] = (byte)value[i].Rotation;
                }
            }
        }

        public MCF1(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public override void ParseData()
        {
            // Loop through each repeating group and parse out pieces of data that matter
            List<MCF1Data> allFontData = new List<MCF1Data>();
            int curIndex = 4;
            int skip = Data[0];

            while (curIndex < Data.Length)
            {
                byte id = Data[curIndex];
                byte fontSectionId = Data[curIndex + 2];
                string cfName = GetReadableDataPiece(curIndex + 4, 8);
                string cpName = GetReadableDataPiece(curIndex + 12, 8);
                string fcsName = GetReadableDataPiece(curIndex + 20, 8);
                CommonMappings.eRotations? rotation = RepeatingGroupLength == 30 && Data.Length >= curIndex + 29 
                    ? (CommonMappings.eRotations?)Data[curIndex + 28] 
                    : null;
                allFontData.Add(new MCF1Data(id, fontSectionId, cfName, cpName, fcsName, rotation));

                curIndex += skip;
            }

            _mappedData = allFontData;
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

        public void AddFontDefinition(string codedFontName, string codePageName, string fontCharSetName)
        {
            // Trim and uppercase everything for easier comparison/insertion
            codedFontName = codedFontName.Trim().ToUpper();
            codePageName = codePageName.Trim().ToUpper();
            fontCharSetName = fontCharSetName.Trim().ToUpper();

            // If it exists already, do nothing
            if (!_mappedData.Any(m => m.CodedFontName.Trim().ToUpper() == codedFontName
                && m.CodePageName.Trim().ToUpper() == codePageName
                && m.FontCharacterSetName.Trim().ToUpper() == fontCharSetName))
            {
                byte newID = (byte)(_mappedData.Count + 1);
                MCF1Data newFontReference = new MCF1Data(newID, 0, codedFontName, codePageName, fontCharSetName);
                _mappedData.Add(newFontReference);
                MappedData = _mappedData; // Updates the data stream in the property
            }
        }

        public class MCF1Data
        {
            public byte ID { get; private set; }
            public byte FontSectionID { get; private set; }
            public string CodedFontName { get; private set; }
            public string CodePageName { get; private set; }
            public string FontCharacterSetName { get; private set; }
            public CommonMappings.eRotations? Rotation { get; private set; }

            public MCF1Data(byte id, byte fontSectionId, string cfName, string cpName, string fcsName, CommonMappings.eRotations? rotation = null)
            {
                ID = id;
                FontSectionID = fontSectionId;
                CodedFontName = cfName;
                CodePageName = cpName;
                FontCharacterSetName = fcsName;
                Rotation = rotation;
            }
        }
    }
}