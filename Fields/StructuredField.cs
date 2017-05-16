using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace AFPParser
{
    [DebuggerDisplay("{string.IsNullOrWhiteSpace(Abbreviation) ? HexCode : Abbreviation}")]
    public abstract class StructuredField
    {
        // Properties directly converted from raw hex data
        public int Length { get; set; }
        public string HexCode { get; set; }
        public byte Flag { get; set; }
        public int Sequence { get; set; }
        public byte[] Data { get; set; }
        
        // Abstract properties derived from hardcoded individual structured field information
        public abstract string Abbreviation { get; }
        public SemanticsInfo Semantics { get; private set; }

        // Dynamically calculated properties
        public string DataHex { get { return BitConverter.ToString(Data).Replace("-", " "); } }
        public string DataEBCDIC { get { return Encoding.GetEncoding("IBM037").GetString(Data); } }

        public StructuredField()
        {
            Semantics = LoadSemantics();
        }

        protected abstract SemanticsInfo LoadSemantics();

        public string BuildDescription()
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrWhiteSpace(Semantics.Description))
            {
                sb.AppendLine("Not yet implemented...");
                sb.AppendLine();
                sb.AppendLine("Raw data:");
                sb.AppendLine(DataHex);
                sb.AppendLine();
                sb.AppendLine("Raw data (EBCDIC):");
                sb.Append(DataEBCDIC);

                return sb.ToString();
            }
            else
            {
                sb.AppendLine(Semantics.Description);
                sb.AppendLine();

                // If this is a repeating group identifier, loop through each subsection of offsets
                int sectionIncrement = 1;
                int rgStartOffset = Semantics.RepeatingGroupStart;
                int skip = rgStartOffset;
                int rgLength = Data.Length;
                do
                {
                    if (Semantics.IsRepeatingGroup)
                    {
                        sb.AppendLine($"SECTION {sectionIncrement++}");
                    }

                    for (int i = 0; i < Semantics.Offsets.Count; i++)
                    {
                        List<Offset> offsetData = Semantics.Offsets;

                        // Get the rest of the data, skipping already processed info
                        byte[] skippedData = Data.Skip(skip).ToArray();

                        // Get the length of the current repeating group (or just the length of the data if not repeating)
                        if (Semantics.IsRepeatingGroup)
                        {
                            // If it is fixed, it will be the first byte of data overall. Otherwise, it will be the first byte of the current segment
                            rgLength = rgStartOffset == 0 ? skippedData[0] : Data[0];

                            // If this offset is past the current RG length, break out of the loop
                            if (offsetData[i].StartingIndex >= rgLength) break;
                        }

                        // Calculate section of bytes to take from data
                        string nextOffsetIdx = i == offsetData.Count - 1 ? "n" : offsetData[i + 1].StartingIndex.ToString();
                        int take = (nextOffsetIdx == "n" ? rgLength : int.Parse(nextOffsetIdx)) - offsetData[i].StartingIndex;
                        byte[] sectionedData = skippedData.Take(take).ToArray();

                        // Build description by datatype
                        switch (offsetData[i].DataType)
                        {
                            // Call our custom parser with this hex code
                            case "CUSTOM":
                                sb.AppendLine(CustomDataParser.ParseData(HexCode, offsetData[i], sectionedData));
                                break;

                            // For triplets, call the triplet parser
                            case "TRIPS":
                                if (i > 0) sb.AppendLine();
                                sb.AppendLine(Triplets.ParseTriplets(sectionedData));
                                break;

                            // Everything else
                            default:
                                // Skip non descriptive offsets
                                if (!string.IsNullOrWhiteSpace(offsetData[i].Description))
                                {
                                    // Write out this offset's description
                                    sb.AppendLine(offsetData[i].DisplayDataByType(sectionedData));
                                }
                                break;
                        }

                        // Increment the skip value so we take the correct offset of data next iteration
                        skip += take;
                    }

                    sb.AppendLine();

                } while (skip < Data.Length);

                return sb.ToString().Trim();
            }
        }
    }
}
