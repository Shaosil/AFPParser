using System;
using System.Text;
using AFPParser.Containers;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
	public class IPD : StructuredField
	{
		private static string _abbr = "IPD";
		private static string _title = "Image Picture Data";
		private static string _desc = "The Image Picture Data structured field contains the data for an image data object.";
		private static List<Offset> _oSets = new List<Offset>();

		public override string Abbreviation => _abbr;
		public override string Title => _title;
		protected override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		protected override List<Offset> Offsets => _oSets;

		public IPD(int length, string hex, byte flag, int sequence) : base (length, hex, flag, sequence) { }

        public override string GetFullDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Description);
            sb.AppendLine();
            sb.AppendLine("Since image data can be split across multiple IPD fields, it");
            sb.AppendLine("has all been parsed into the container as one data stream.");
            sb.AppendLine();
            sb.AppendLine($"Raw image hex data:");
            sb.AppendLine(BitConverter.ToString(((ImageObjectContainer)LowestLevelContainer).ImageData).Replace("-", " "));

            return sb.ToString();
        }
    }
}