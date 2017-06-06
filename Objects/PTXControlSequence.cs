using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AFPParser
{
    public abstract class PTXControlSequence : DataStructure
    {
        // Properties which must be implemented by individual control sequences
        public abstract string Abbreviation { get; }
        protected abstract string Description { get; }
        protected abstract List<Offset> Offsets { get; }    // Keep in mind that offset 0 in code is actually offset 4, since the first four bytes are always the same
        protected override string StructureName => "Control Sequence";

        public PTXControlSequence(byte[] allData) : base(allData[0], allData[1].ToString("X2"), 2)
        {
            // Control sequences never have repeating groups
            Semantics = new SemanticsInfo(SpacedClassName, Description, false, 0, Offsets);

            // Set data starting at offset 2
            for (int i = 0; i < Data.Length; i++)
                Data[i] = allData[2 + i];
        }

        public override string GetFullDescription()
        {
            StringBuilder sb = new StringBuilder();

            // Use description instead of title
            sb.AppendLine($"{Semantics.Description} ({StructureName} 0x{ID})");
            sb.Append(GetOffsetDescriptions());

            return sb.ToString();
        }

        public override void ParseData()
        {
            // TODO: Remove this if and when each control sequence parses the data in their own way
        }

        public static List<PTXControlSequence> GetCSIs(byte[] csData)
        {
            // PTX Data is made up of Control Sequences, which can be chained/unchained
            List<PTXControlSequence> csiList = new List<PTXControlSequence>();

            // The first two bytes of the CSI data are always 2B D3, so skip them
            int curIndex = 2;
            while (curIndex < csData.Length)
            {
                // Get the one byte length
                int length = csData[curIndex];
                byte CSTypeByte = csData[curIndex + 1];

                // Get our current CSI by length
                byte[] sectionedCSI = csData.Skip(curIndex).Take(length).ToArray();

                // Build and add the sequence by data type
                Type CSType = typeof(PTXControlSequences.UNKNOWN);
                if (Lookups.PTXControlSequences.ContainsKey(CSTypeByte))
                    CSType = Lookups.PTXControlSequences[CSTypeByte];
                PTXControlSequence sequence = (PTXControlSequence)Activator.CreateInstance(CSType, sectionedCSI);
                csiList.Add(sequence);

                // Skip an extra 2 bytes if the CSI we just read is unchained, since the next CSI will contain the prefixes
                curIndex += length + (CSTypeByte % 2 == 1 ? 0 : 2);
            }

            // Parse all data
            foreach (PTXControlSequence sequence in csiList)
                sequence.ParseData();

            return csiList;
        }
    }
}