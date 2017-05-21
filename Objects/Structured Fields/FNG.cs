using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class FNG : StructuredField
    {
        private static string _abbr = "FNG";
        private static string _title = "Font Patterns";
        private static string _desc = "Carries the character shape data (raster patterns or outline data) for a font character set.";
        private static List<Offset> _oSets = new List<Offset>()
        {

        };

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        protected override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        protected override List<Offset> Offsets => _oSets;

        public FNG(int length, string hex, byte flag, int sequence) : base(length, hex, flag, sequence) { }
    }
}