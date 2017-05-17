using System;
using System.Linq;
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
        protected override string Description => _desc;
        protected override bool IsRepeatingGroup => false;
        protected override int RepeatingGroupStart => 0;
        protected override List<Offset> Offsets => _oSets;
        
        public List<PTXControlSequence> CSIs { get; set; }

        public PTX(int length, string hex, byte flag, int sequence) : base(length, hex, flag, sequence) { }

        protected override string BuildOffsetDescriptions()
        {
            StringBuilder sb = new StringBuilder();

            // PTX Data is made up of Control Sequences, which can be chained/unchained
            CSIs = GetCSIs();

            // Get the description of each offset in each sequence
            foreach (PTXControlSequence sequence in CSIs)
            {
                // Write out the info
                sb.AppendLine(sequence.GetDescription());
            }

            return sb.ToString();
        }

        private List<PTXControlSequence> GetCSIs()
        {
            CSIs = new List<PTXControlSequence>();

            // The first two bytes of the CSI data are always 2B D3, so skip them
            int curIndex = 2;
            while (curIndex < Data.Length)
            {
                // Get the one byte length
                int length = Data[curIndex];
                byte CSTypeByte = Data[curIndex + 1];

                // Get our current CSI by length
                byte[] sectionedCSI = Data.Skip(curIndex).Take(length).ToArray();

                // Build and add the sequence by data type
                Type CSType = typeof(PTXControlSequences.UNKNOWN);
                if (Lookups.PTXControlSequences.ContainsKey(CSTypeByte))
                    CSType = Lookups.PTXControlSequences[CSTypeByte];
                PTXControlSequence sequence = (PTXControlSequence)Activator.CreateInstance(CSType, sectionedCSI);
                CSIs.Add(sequence);

                // Skip an extra 2 bytes if the CSI we just read is unchained, since the next CSI will contain the prefixes
                curIndex += length + (CSTypeByte % 2 == 1 ? 0 : 2);
            }

            return CSIs;
        }
    }
}