using System;
using System.Linq;
using System.Text;
using AFPParser.Properties;
using System.Collections.Generic;

namespace AFPParser
{
    public class Triplets
    {
        public static Dictionary<byte, SemanticsInfo> All { get; private set; }

        public static void Load()
        {
            Triplets.All = new Dictionary<byte, SemanticsInfo>();

            // Load the list of triplet semantics into memory
            List<string> tripletFileLines = Resources.Triplets.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            int curIdx = 0, nextIdx = 0;
            while (curIdx >= 0)
            {
                curIdx = tripletFileLines.IndexOf(tripletFileLines.Skip(curIdx).FirstOrDefault(i => i[0] == ':'));
                nextIdx = tripletFileLines.IndexOf(tripletFileLines.Skip(curIdx + 1).FirstOrDefault(i => i[0] == ':'));

                // Create new dictionary entry with current triplet info
                SemanticsInfo semantics = new SemanticsInfo();
                byte convertedByte = byte.Parse(tripletFileLines[curIdx].Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                Triplets.All.Add(convertedByte, semantics);

                // Set title and description based on constant line number
                if (curIdx + 1 < tripletFileLines.Count)
                    semantics.Title = tripletFileLines[curIdx + 1];
                if (curIdx + 2 < tripletFileLines.Count)
                    semantics.Description = tripletFileLines[curIdx + 2];
                curIdx += 3;

                // Get the list of lines that are offsets
                List<string> offsetRows = new List<string>();
                for (int i = curIdx; i < tripletFileLines.Count; i++)
                {
                    if (tripletFileLines[i][0] == ':') break;
                    offsetRows.Add(tripletFileLines[i]);
                }

                semantics.Offsets = Parser.LoadOffsets(offsetRows, semantics);

                curIdx = nextIdx;
            }
        }

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
                        // Get sectioned data
                        byte[] sectionedData = triplet.Skip(oSet.StartingIndex).ToArray();
                        if (oSet != tripletInfo.Offsets.Last())
                        {
                            int nextIndex = tripletInfo.Offsets.IndexOf(oSet) + 1;
                            int bytesToTake = tripletInfo.Offsets[nextIndex].StartingIndex - oSet.StartingIndex;
                            sectionedData = sectionedData.Take(bytesToTake).ToArray();
                        }

                        // Write out this offset's description
                        sb.AppendLine(oSet.DisplayDataByType(sectionedData));
                    }
                }
            }

            return sb.ToString();
        }
    }
}