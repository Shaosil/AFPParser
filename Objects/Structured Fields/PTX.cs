using System.Text;
using System.Collections.Generic;

namespace AFPParser.StructuredFields
{
    public class PTX : StructuredField
    {
        private static string _abbr = "PTX";
        private static string _title = "Presentation Text Data";
        private static string _desc = "Contains the data for a presentation text data object.";
        private static List<Offset> _oSets = new List<Offset>();

        public override string Abbreviation => _abbr;
        public override string Title => _title;
        public override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        public override IReadOnlyList<Offset> Offsets => _oSets;

        public List<PTXControlSequence> CSIs { get; set; }

        public PTX(int length, string hex, byte flag, int sequence) : base(length, hex, flag, sequence) { }

        protected override string GetOffsetDescriptions()
        {
            StringBuilder sb = new StringBuilder();

            // Get the description of each offset in each sequence
            foreach (PTXControlSequence sequence in CSIs)
                sb.AppendLine(sequence.GetFullDescription());

            return sb.ToString();
        }

        public override void ParseData()
        {
            CSIs = PTXControlSequence.GetCSIs(Data);
        }
    }
}