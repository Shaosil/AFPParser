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
        protected abstract List<Offset> Offsets { get; }
        
        public PTXControlSequence(byte[] allData) : base (allData[0], allData[1].ToString("X"), 2)
        {
            // Control sequences never have repeating groups
            Semantics = new SemanticsInfo(SpacedClassName, Description, false, 0, Offsets);

            // Set data starting at offset 2
            for (int i = 0; i < Data.Length; i++)
                Data[i] = allData[2 + i];
        }

        // Complicated control sequences can override this method
        public string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine($"{Semantics.Title} (Control Sequence 0x{ID})");
            sb.Append(GetOffsetDescriptions());

            return sb.ToString();
        }

        protected virtual string GetOffsetDescriptions()
        {
            StringBuilder sb = new StringBuilder();

            if (Semantics.Offsets.Any())
                foreach (Offset oSet in Offsets)
                {
                    if (!string.IsNullOrWhiteSpace(oSet.Description))
                    {
                        // Get sectioned data
                        byte[] sectionedData = Data.Skip(oSet.StartingIndex).ToArray();
                        if (oSet != Offsets.Last())
                        {
                            int nextIndex = Offsets.IndexOf(oSet) + 1;
                            int bytesToTake = Offsets[nextIndex].StartingIndex - oSet.StartingIndex;
                            sectionedData = sectionedData.Take(bytesToTake).ToArray();
                        }

                        // Write out this offset's description
                        sb.AppendLine(oSet.DisplayDataByType(sectionedData));
                    }
                }
            else
                sb.AppendLine($"Not yet implemented...{Environment.NewLine}Raw Data: {DataHex}");

            return sb.ToString();
        }
    }
}