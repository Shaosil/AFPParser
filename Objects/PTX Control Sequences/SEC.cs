using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace AFPParser.PTXControlSequences
{
	public class SEC : PTXControlSequence
	{
		private static string _abbr = "SEC";
		private static string _desc = "Set Extended Text Color";
        private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.EMPTY, ""),
            new Offset(1, Lookups.DataTypes.CODE, "Color Space")
            {
                Mappings = new Dictionary<byte, string>()
                {
                    { 0x01, eColorSpace.RGB.ToString() },
                    { 0x04, eColorSpace.CMYK.ToString() },
                    { 0x06, eColorSpace.Highlight.ToString() },
                    { 0x08, eColorSpace.CIELAB.ToString() },
                    { 0x40, eColorSpace.StandardOCA.ToString() }
                }
            },
            new Offset(2, Lookups.DataTypes.EMPTY, ""),
            new Offset(6, Lookups.DataTypes.UBIN, "Component 1 bit count"),
            new Offset(7, Lookups.DataTypes.UBIN, "Component 2 bit count"),
            new Offset(8, Lookups.DataTypes.UBIN, "Component 3 bit count"),
            new Offset(9, Lookups.DataTypes.UBIN, "Component 4 bit count"),
            new Offset(10, Lookups.DataTypes.EMPTY, "Color Specs - CUSTOM PARSED")
        };

        public override string Abbreviation => _abbr;
		public override string Description => _desc;
		public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed data
        public enum eColorSpace { RGB, CMYK, Highlight, CIELAB, StandardOCA };
        public eColorSpace ColorSpace { get; private set; }
        public int C1BitCount { get; private set; }
        public int C2BitCount { get; private set; }
        public int C3BitCount { get; private set; }
        public int C4BitCount { get; private set; }
        public Color TextColor { get; private set; }

        public SEC(byte[] data) : base(data) { }

        public override void ParseData()
        {
            ColorSpace = (eColorSpace)Enum.Parse(typeof(eColorSpace), _oSets[1].Mappings[Data[1]]);
            C1BitCount = Data[6];
            C2BitCount = Data[7];
            C3BitCount = Data[8];
            C4BitCount = Data[9];

            // Get the color value based on the color space and component values
            switch (ColorSpace)
            {
                case eColorSpace.RGB:
                    TextColor = Color.FromArgb(Data[10], Data[11], Data[12]);
                    break;

                case eColorSpace.CMYK:
                    // CMYK are maxed 0 to 1, which must be converted from the binary value range 0 to (bitCount*2 - 1)
                    int[] byteCounts = new int[4] { C1BitCount / 8, C2BitCount / 8, C3BitCount / 8, C4BitCount / 8 };
                    int[] maxValues = new int[4] { (2 * (C1BitCount) - 1), (2 * (C2BitCount) - 1), (2 * (C3BitCount) - 1), (2 * (C4BitCount) - 1) };
                    int[] values = new int[4];
                    for (int i = 0; i < 4; i++)
                    {
                        // Get each value
                        int startAt = 10 + byteCounts.Take(i + 1).Sum();
                        values[i] = (int)GetNumericValue(GetSectionedData(startAt, byteCounts[i]), false);
                    }

                    /*
                    The red (R) color is calculated from the cyan (C) and black (K) color
                    R = 255 * (1-C) * (1-K)

                    The green color (G) is calculated from the magenta (M) and black (K) colors:
                    G = 255 * (1-M) * (1-K)

                    The blue color (B) is calculated from the yellow (Y) and black (K) colors:
                    B = 255 * (1-Y) * (1-K)
                    */

                    int red = 255 * (1 - values[0]) * (1 - values[3]);
                    int green = 255 * (1 - values[1]) * (1 - values[3]);
                    int blue = 255 * (1 - values[2]) * (1 - values[3]);
                    TextColor = Color.FromArgb(red, green, blue);

                    break;

                case eColorSpace.StandardOCA:
                    // Preset table
                    if (Lookups.StandardOCAColors.ContainsKey(Data[11]))
                        TextColor = Lookups.StandardOCAColors[Data[11]];
                    else
                        TextColor = Color.Black;

                    break;
            }
        }

        protected override string GetSingleOffsetDescription(Offset oSet, byte[] sectionedData)
        {
            if (oSet.StartingIndex != 10)
                return base.GetSingleOffsetDescription(oSet, sectionedData);

            StringBuilder sb = new StringBuilder();
            if (TextColor != null)
                sb.AppendLine(TextColor.ToString());
            else
                sb.AppendLine($"Unsupported color space. Raw Data: {BitConverter.ToString(sectionedData).Replace("-", " ")}");
            return sb.ToString();
        }
    }
}
