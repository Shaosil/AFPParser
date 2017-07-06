using System.Text;
using System.Linq;
using System.Collections.Generic;
using System;

namespace AFPParser.StructuredFields
{
    public class CPI : StructuredField
    {
        private static string _abbr = "CPI";
        private static string _title = "Code Page Index";
        private static string _desc = "Associates one or more character IDs with code points.";

        // This incomplete set of offsets will be parsed manually, but store them for ease of use
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.CHAR, "Graphic Character GID"),
            new Offset(8, Lookups.DataTypes.BITS, "Graphic Character Use Flags") { Mappings = CommonMappings.CharacterUseFlags },
            new Offset(9, Lookups.DataTypes.EMPTY, "Code Point")
        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => true;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public IReadOnlyList<Info> CPIInfos { get; private set; }

        public CPI(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public override void ParseData()
        {
            int curIndex = 0;
            CPC cpcField = LowestLevelContainer.GetStructure<CPC>();
            List<Info> allInfo = new List<Info>();

            while (curIndex < Data.Length)
            {
                string gid = GetReadableDataPiece(curIndex + 0, 8);
                byte[] codePoints = GetSectionedData(curIndex + 9, cpcField.IsSingleByteCodePage ? 1 : 2);
                allInfo.Add(new Info(gid, codePoints));

                // 10 bytes RG Length for single bytes, 11 for double
                curIndex += cpcField.IsSingleByteCodePage ? 10 : 11;
            }

            CPIInfos = allInfo;
        }

        protected override string GetOffsetDescriptions()
        {
            StringBuilder sb = new StringBuilder();

            // CPI will have one or more repeating groups. The length of each is found in the CPC field
            CPC cpcField = LowestLevelContainer.GetStructure<CPC>();

            // Single byte code points are length 10, double are length 11
            int standardLength = cpcField.IsSingleByteCodePage ? 10 : 11;

            // Loop through however many sections we need to
            for (int curIndex = 0; curIndex < Data.Length;)
            {
                // Retrieve the byte sections
                byte[] GCGID = GetSectionedData(curIndex, 8);
                byte[] PrtFlags = GetSectionedData(curIndex + 8, 1);
                byte[] CodePoint = GetSectionedData(curIndex + 9, cpcField.IsSingleByteCodePage ? 1 : 2);

                // Display first 3 semantics based on predefined offsets above
                sb.AppendLine(Offsets[0].DisplayDataByType(GCGID));
                sb.AppendLine(Offsets[1].DisplayDataByType(PrtFlags));
                sb.AppendLine(Offsets[2].DisplayDataByType(CodePoint));

                // If this code point includes Unicode scalar value entries, parse them here
                if (!cpcField.IsSingleByteCodePage)
                {
                    int numScalarValues = Data[standardLength];

                    for (int i = 0; i < numScalarValues; i += 4)
                    {
                        Offset fakeOffset = new Offset(0, Lookups.DataTypes.UBIN, $"Unicode Scalar Value {i + 1}");

                        // Each scalar value is a four byte UBIN
                        int startingIndex = standardLength + (i * 4);
                        byte[] scalarValue = GetSectionedData(startingIndex, 4);
                        sb.AppendLine(fakeOffset.DisplayDataByType(scalarValue));
                    }

                    // Take extra semantics into account
                    curIndex += 1 + (numScalarValues * 4);
                }
                sb.AppendLine();

                // Go to the next one
                curIndex += standardLength;
            }

            return sb.ToString();
        }

        public class Info
        {
            public string GID { get; private set; }
            public byte[] CodePoints { get; private set; } // Single byte or double byte code points

            public Info(string gid, byte[] codePoints)
            {
                GID = gid;
                CodePoints = codePoints;
            }
        }
    }
}