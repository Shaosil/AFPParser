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
		public override string Description => _desc;
		protected override bool IsRepeatingGroup => false;
		protected override int RepeatingGroupStart => 0;
		public override IReadOnlyList<Offset> Offsets => _oSets;

		public IPD(string id, byte[] introducer, byte[] data) : base(id, introducer, data) { }

        public override string GetFullDescription()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Description);
            sb.AppendLine();
            sb.AppendLine("Since image data can be split across multiple IPD fields, it");
            sb.AppendLine("has all been parsed into the container as one data stream.");
            sb.AppendLine();

            int count = 1;
            foreach (ImageContentContainer.ImageInfo info in ((IOCAImageContainer)LowestLevelContainer).Images)
            {
                sb.AppendLine($"Raw image {count} data:");
                sb.AppendLine(BitConverter.ToString(info.Data).Replace("-", " "));
                if (info.TransparencyMask.Length > 0)
                {
                    sb.AppendLine($"Raw image {count++} transparency data:");
                    sb.AppendLine(BitConverter.ToString(info.TransparencyMask).Replace("-", " "));
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}