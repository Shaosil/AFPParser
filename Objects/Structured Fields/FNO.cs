using System.Text;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Collections;

namespace AFPParser.StructuredFields
{
    public class FNO : StructuredField
    {
        private static string _abbr = "FNO";
        private static string _title = "Font Orientation";
        private static string _desc = "Each group in this field applies to a single character rotation of a font character set.";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, ""),
            new Offset(2, Lookups.DataTypes.UBIN, "Character Rotation") { Mappings = CommonMappings.Rotations },
            new Offset(4, Lookups.DataTypes.SBIN, "Max Baseline Offset"),
            new Offset(6, Lookups.DataTypes.UBIN, "Max Character Increment"),
            new Offset(8, Lookups.DataTypes.UBIN, "Space Character Increment"),
            new Offset(10, Lookups.DataTypes.UBIN, "Max Baseline Extent"),
            new Offset(12, Lookups.DataTypes.BITS, "CUSTOM PARSED! SHOULD NOT SEE THIS TEXT"),
            new Offset(13, Lookups.DataTypes.EMPTY, ""),
            new Offset(14, Lookups.DataTypes.UBIN, "Em-Space Increment"),
            new Offset(16, Lookups.DataTypes.EMPTY, ""),
            new Offset(18, Lookups.DataTypes.UBIN, "Figure Space Increment"),
            new Offset(20, Lookups.DataTypes.UBIN, "Nominal Character Increment"),
            new Offset(22, Lookups.DataTypes.UBIN, "Default Baseline Increment"),
            new Offset(24, Lookups.DataTypes.SBIN, "Minimum A-Space")
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
                    length = control.Data[14];

                return length;
            }
        }
        public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public IReadOnlyList<Info> FNOInfo { get; private set; }

        public FNO(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data) { }

        public override void ParseData()
        {
            // Get all groups
            int curIndx = 0;
            List<Info> allInfo = new List<Info>();

            while (curIndx < Data.Length)
            {
                allInfo.Add(new Info((int)GetNumericValue(GetSectionedData(curIndx + 8, 2), false)));
                curIndx += RepeatingGroupLength;
            }

            FNOInfo = allInfo;
        }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            StringBuilder sb = new StringBuilder();

            // Parse everything except offset 12 - handle that one ourselves
            if (oSet.StartingIndex != 12)
                sb.Append(base.GetSingleOffsetDescription(oSet, sectionedData));
            else
            {
                sb.AppendLine($"Control Flags:");

                // The first three bits represent a numeric value, so add 5 blank bytes
                BitArray indexNumBits = new BitArray(new bool[5] { false, false, false, false, false }
                    .Concat(GetBitArray(sectionedData).Take(3)).ToArray());
                int[] FNINumInt = new int[1];
                indexNumBits.CopyTo(FNINumInt, 0);
                sb.AppendLine($"* Font Index Number: {FNINumInt[0]}");

                sb.Append("* ");
                if ((sectionedData[0] & (1 << 3)) == 0) sb.Append("No ");
                sb.AppendLine("Kerning Data");

                sb.Append("* ");
                if ((sectionedData[0] & (1 << 2)) == 0) sb.Append("Minimum ");
                else sb.Append("Uniform ");
                sb.AppendLine("A-space");

                sb.Append("* ");
                if ((sectionedData[0] & (1 << 1)) == 0) sb.Append("Maximum ");
                else sb.Append("Uniform ");
                sb.AppendLine("baseline offset");

                sb.Append("* ");
                if ((sectionedData[0] & 1) == 0) sb.Append("Maximum ");
                else sb.Append("Uniform ");
                sb.AppendLine("character increment");
            }

            return sb.ToString();
        }

        public class Info
        {
            public int SpaceCharIncrement { get; private set; }

            public Info(int spaceCharInc)
            {
                SpaceCharIncrement = spaceCharInc;
            }
        }
    }
}