using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AFPParser
{
    public abstract class Triplet : DataStructure
    {
        // Properties which must be implemented by individual triplets
        protected abstract string Description { get; }
        protected abstract List<Offset> Offsets { get; }
        protected override string StructureName => "Triplet";
        
        public Triplet(byte[] allData) : base (allData[0], allData[1].ToString("X"), 2)
        {
            // Triplets never have repeating groups
            Semantics = new SemanticsInfo(SpacedClassName, Description, false, 0, Offsets);

            // Set data starting at offset 2
            for (int i = 0; i < Data.Length; i++)
                Data[i] = allData[2 + i];
        }

        public static string ParseAll(byte[] tripletData)
        {
            StringBuilder sb = new StringBuilder();

            List<byte[]> triplets = new List<byte[]>();
            int curIndex = 0;
            while (curIndex < tripletData.Length)
            {
                // The first byte is always length, so use that to add each triplet section to the list of byte arrays
                int length = tripletData[curIndex];

                // Add the current triplet to the list
                triplets.Add(tripletData.Skip(curIndex).Take(length).ToArray());

                curIndex += length;
            }

            // Append each triplet's description and data
            foreach (byte[] triplet in triplets)
            {
                // Get the type of triplet
                Type tripletType = typeof(Triplets.UNKNOWN);
                if (Lookups.Triplets.ContainsKey(triplet[1]))
                    tripletType = Lookups.Triplets[triplet[1]];
                Triplet assignedTriplet = (Triplet)Activator.CreateInstance(tripletType, triplet);

                sb.AppendLine(assignedTriplet.GetDescription());
            }

            return sb.ToString();
        }

        protected override string GetOffsetDescriptions()
        {
            StringBuilder sb = new StringBuilder();

            if (Offsets.Any())
                foreach (Offset oSet in Offsets)
                {
                    if (!string.IsNullOrWhiteSpace(oSet.Description))
                    {
                        // Get sectioned data
                        int nextIndex = Offsets.IndexOf(oSet) + 1;
                        int bytesToTake = oSet == Offsets.Last()
                            ? Data.Length - oSet.StartingIndex
                            : Offsets[nextIndex].StartingIndex - oSet.StartingIndex;
                        byte[] sectionedData = GetSectionedData(oSet.StartingIndex, bytesToTake);

                        // Write out this offset's description
                        sb.AppendLine(oSet.DisplayDataByType(sectionedData));
                    }
                }
            else
            {
                sb.AppendLine($"Not yet implemented...{Environment.NewLine}Raw Data: {DataHex}");
            }

            return sb.ToString();
        }
    }
}