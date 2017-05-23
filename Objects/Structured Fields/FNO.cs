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
            new Offset(2, Lookups.DataTypes.UBIN, "Character Rotation")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x00, "0 Degrees" },
                    { 0x2D, "90 Degrees" },
                    { 0x5A, "180 Degrees" },
                    { 0x87, "270 Degrees" }
                }
            },
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
        protected override string Description => _desc;
        protected override bool IsRepeatingGroup => true;
        protected override int RepeatingGroupStart => 0;
        protected override int RepeatingGroupLength
        {
            get
            {
                int length = Data.Length;
                FNC control = (FNC)Parser.AfpFile.FirstOrDefault(f => f.GetType() == typeof(FNC));
                if (control != null)
                    length = control.Data[14];

                return length;
            }
        }
        protected override List<Offset> Offsets => _oSets;

        public FNO(int length, string hex, byte flag, int sequence) : base(length, hex, flag, sequence) { }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            StringBuilder sb = new StringBuilder();

            // Parse everything except offset 12 - handle that one ourselves
            if (oSet.StartingIndex != 12)
                sb.Append(base.GetSingleOffsetDescription(oSet, sectionedData));
            else
            {
                sb.AppendLine($"Control Flags:");

                bool[] bitArray = GetBitArray(Data[12]);

                // The first three bits represent a numeric value
                bool[] FNINum = bitArray.Take(3).ToArray();
                if (BitConverter.IsLittleEndian) FNINum = FNINum.Reverse().ToArray();
                int[] FNINumInt = new int[1];
                new BitArray(FNINum).CopyTo(FNINumInt, 0);
                sb.AppendLine($"* Font Index Number: {FNINumInt[0]}");

                sb.Append("* ");
                if (!bitArray[4]) sb.Append("No ");
                sb.AppendLine("Kerning Data");

                sb.Append("* ");
                if (!bitArray[5]) sb.Append("Minimum ");
                else sb.Append("Uniform ");
                sb.AppendLine("A-space");

                sb.Append("* ");
                if (!bitArray[6]) sb.Append("Maximum ");
                else sb.Append("Uniform ");
                sb.AppendLine("baseline offset");

                sb.Append("* ");
                if (!bitArray[7]) sb.Append("Maximum ");
                else sb.Append("Uniform ");
                sb.AppendLine("character increment");
            }

            return sb.ToString();
        }
    }
}