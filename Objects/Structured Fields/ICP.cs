using System;
using System.Linq;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class ICP : StructuredField
	{
		private static string _abbr = "ICP";
		private static string _title = "Image Cell Position";
		private static string _desc = "The IM Image Cell Position structured field specifies the placement, size, and replication of IM image cells.";
		private static List<Offset> _oSets = new List<Offset>()
        {
            new Offset(0, Lookups.DataTypes.UBIN, "X Offset"),
            new Offset(2, Lookups.DataTypes.UBIN, "Y Offset"),
            new Offset(4, Lookups.DataTypes.UBIN, "X Size"),
            new Offset(6, Lookups.DataTypes.UBIN, "Y Size"),
            new Offset(8, Lookups.DataTypes.UBIN, "Size of X Fill Rectangle"),
            new Offset(10, Lookups.DataTypes.UBIN, "Size of Y Fill Rectangle")
        };

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

        // Parsed Data
        public int XOffset { get; private set; }
        public int YOffset { get; private set; }
        public int XSize { get; private set; }
        public int YSize { get; private set; }
        public int XFillSize { get; private set; }
        public int YFillSize { get; private set; }

        public ICP(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        // Realtime use
        public ICP(ushort xSize, ushort ySize) : base(new byte[3] { 0xD3, 0xAC, 0x7B }, 0, 0, new byte[0])
        {
            Data = new byte[12];
            byte[] xSizeBytes = BitConverter.GetBytes(xSize);
            byte[] ySizeBytes = BitConverter.GetBytes(ySize);
            if (BitConverter.IsLittleEndian)
            {
                xSizeBytes = xSizeBytes.Reverse().ToArray();
                ySizeBytes = ySizeBytes.Reverse().ToArray();
            }

            // Fill data with X and Y sizing information
            Array.ConstrainedCopy(xSizeBytes, 0, Data, 4, 2);
            Array.ConstrainedCopy(ySizeBytes, 0, Data, 6, 2);
            Array.ConstrainedCopy(xSizeBytes, 0, Data, 8, 2);
            Array.ConstrainedCopy(ySizeBytes, 0, Data, 10, 2);

            ParseData();
        }

        public override void ParseData()
        {
            XOffset = (int)GetNumericValue(GetSectionedData(0, 2), false);
            YOffset = (int)GetNumericValue(GetSectionedData(2, 2), false);
            XSize = (int)GetNumericValue(GetSectionedData(4, 2), false);
            YSize = (int)GetNumericValue(GetSectionedData(6, 2), false);
            XFillSize = (int)GetNumericValue(GetSectionedData(8, 2), false);
            YFillSize = (int)GetNumericValue(GetSectionedData(10, 2), false);
        }
    }
}