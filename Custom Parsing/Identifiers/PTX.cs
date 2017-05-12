using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace AFPParser
{
    public partial class CustomDataParser
    {
        private static string XD3EE9B(Offset oSet, byte[] data)
        {
            StringBuilder sb = new StringBuilder();

            // PTX Data is made up of Control Sequences, which can be chained/unchained

            // Get the description of each offset in each sequence
            List<PTX.ControlSequence> sequences = new PTX(data).CSIs;
            foreach (PTX.ControlSequence sequence in sequences)
            {
                // Grab sequence info for this function type
                SemanticsInfo sequenceInfo =
                    PTXCSIFunctions.All.ContainsKey(sequence.FuncType)
                        ? PTXCSIFunctions.All[sequence.FuncType]
                        : new SemanticsInfo() { Description = $"UNKNOWN SEQUENCE FUNCTION (0x{sequence.FuncType.ToString("X")})." };

                // Write out the info
                sb.AppendLine(sequenceInfo.Description);

                if (sequenceInfo.Offsets.Any())
                    foreach (Offset sOSet in sequenceInfo.Offsets)
                    {
                        // Parse custom data if needed
                        if (sOSet.DataType == "CUSTOM")
                        {
                            // Gather both chained versions of this function so we can properly find the reflected method info
                            List<byte> relatedBytes = PTXCSIFunctions.All.Where(p => p.Value == sequenceInfo).Select(p => p.Key).ToList();
                            relatedBytes.Sort();
                            string hexValue = $"{string.Join("_", relatedBytes.Select(b => b.ToString("X")))}_PTX";
                            sb.AppendLine(ParseData(hexValue, sOSet, sequence.Data));
                        }
                        else
                        {
                            // Get sectioned data and parse it by offset info
                            int nextIndex = sOSet != sequenceInfo.Offsets.Last()
                                ? sequenceInfo.Offsets[sequenceInfo.Offsets.IndexOf(sOSet) + 1].StartingIndex
                                : sequence.Data.Length;
                            int bytesToTake = nextIndex - sOSet.StartingIndex;
                            byte[] sectionedData = sequence.Data.Skip(sOSet.StartingIndex).Take(bytesToTake).ToArray();
                            sb.AppendLine(sOSet.DisplayDataByType(sequence.Data));
                        }
                    }
                else
                    sb.AppendLine("Not yet implemented...");

                sb.AppendLine();
            }

            return sb.ToString();
        }

        private class PTX
        {
            public List<ControlSequence> CSIs { get; set; }

            public PTX(byte[] ptxData)
            {
                CSIs = GetCSIs(ptxData);
            }

            private List<ControlSequence> GetCSIs(byte[] data)
            {
                CSIs = new List<ControlSequence>();

                // The first two bytes of the CSI data are always 2B D3, so skip them
                int curIndex = 2;
                while (curIndex < data.Length)
                {
                    // Get the one byte length
                    int length = data[curIndex];

                    // Get our current CSI by length
                    byte[] sectionedCSI = data.Skip(curIndex).Take(length).ToArray();

                    // Build and add the sequence by data
                    ControlSequence sequence = new ControlSequence(sectionedCSI);
                    CSIs.Add(sequence);

                    // Skip an extra 2 bytes if the CSI we just read is unchained, since the next CSI will contain the prefixes
                    curIndex += length + (sequence.IsChained ? 0 : 2);
                }

                return CSIs;
            }

            [DebuggerDisplay("{FuncType.ToString(\"X\")}")]
            public class ControlSequence
            {
                // If the lower bit is 1, this is a chained function
                public bool IsChained { get { return FuncType % 2 == 1; } }

                public int Length { get; set; }
                public byte FuncType { get; set; }
                public byte[] Data { get; set; }

                public ControlSequence(byte[] data)
                {
                    Length = data[0];
                    FuncType = data[1];
                    Data = data.Skip(2).ToArray();
                }
            }
        }
    }
}