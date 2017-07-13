using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        // Parsed Data
        private List<PTXControlSequence> _controlSequences = new List<PTXControlSequence>();
        public IReadOnlyList<PTXControlSequence> ControlSequences
        {
            get { return _controlSequences; }
            private set
            {
                _controlSequences = value.ToList();

                // Update the data stream
                Data = new byte[value.Sum(v => v.Length)];
                int curIndex = 0;
                for (int i = 0; i < value.Count; i++)
                {
                    // Only need to insert the introducer and data of each PTXControlSequence, since they are data structures as well
                    Array.ConstrainedCopy(value[i].Introducer, 0, Data, curIndex, value[i].Introducer.Length);
                    curIndex += value[i].Introducer.Length;
                    Array.ConstrainedCopy(value[i].Data, 0, Data, curIndex, value[i].Data.Length);
                    curIndex += value[i].Data.Length;
                }
            }
        }

        public PTX(byte[] id, byte flag, ushort sequence, byte[] data) : base(id, flag, sequence, data) { }

        public PTX(List<PTXControlSequence> controlSequences) : base(Lookups.StructuredFieldID<PTX>(), 0, 0, null)
        {
            ControlSequences = controlSequences;
        }

        protected override string GetOffsetDescriptions()
        {
            StringBuilder sb = new StringBuilder();

            // Get the description of each offset in each sequence
            foreach (PTXControlSequence sequence in ControlSequences)
                sb.AppendLine(sequence.GetFullDescription());

            return sb.ToString();
        }

        public override void ParseData()
        {
            _controlSequences = PTXControlSequence.GetCSIs(Data);
        }
    }
}