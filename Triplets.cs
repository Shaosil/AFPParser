using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFPParser
{
    public class Triplets
    {
        public static Dictionary<byte, SemanticsInfo> All = new Dictionary<byte, SemanticsInfo>();

        public static string ParseTriplets(byte[] tripletData)
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
                sb.AppendLine(GetTripletDescription(triplet));

            return sb.ToString();
        }

        private static string GetTripletDescription(byte[] triplet)
        {
            StringBuilder sb = new StringBuilder();

            // Lookup description and data type by triplet hex value
            if (All.ContainsKey(triplet[1]))
            {
                SemanticsInfo tripletInfo = All[triplet[1]];
                string strTriplet = BitConverter.ToString(triplet, 1, 1);
                sb.AppendLine($"{tripletInfo.Title} (Triplet 0x{strTriplet})");

                foreach (Offset oSet in tripletInfo.Offsets)
                {

                    // Call our custom parser if needed
                    if (oSet.DataType == "CUSTOM")
                    {
                        sb.AppendLine(CustomDataParser.ParseData(strTriplet, oSet, triplet));
                    }
                    // Everything else can be handled in this block
                    else if (!string.IsNullOrWhiteSpace(oSet.Description))
                    {
                        sb.Append($"{oSet.Description}: ");

                        // If this offset byte has a mapping list, lookup by that instead
                        if (oSet.Mappings.Any())
                            sb.AppendLine(oSet.DisplayMappedInfo(triplet[oSet.StartingIndex]));
                        else
                        {
                            // Get sectioned data
                            byte[] sectionedData = triplet.Skip(oSet.StartingIndex).ToArray();

                            if (oSet != tripletInfo.Offsets.Last())
                            {
                                int nextIndex = tripletInfo.Offsets.IndexOf(oSet) + 1;
                                int bytesToTake = tripletInfo.Offsets[nextIndex].StartingIndex - oSet.StartingIndex;
                                sectionedData = sectionedData.Take(bytesToTake).ToArray();
                            }
                            sb.AppendLine(oSet.DisplayDataByType(sectionedData));
                        }
                    }
                }
            }

            return sb.ToString();
        }
    }
}