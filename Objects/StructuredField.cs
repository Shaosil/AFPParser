using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace AFPParser
{
    [DebuggerDisplay("{string.IsNullOrWhiteSpace(Abbreviation) ? ID : Abbreviation}")]
    public abstract class StructuredField : DataStructure
    {
        // Properties directly converted from raw hex data
        public byte Flag { get; private set; }
        public int Sequence { get; private set; }
        
        // Abstract properties derived from hardcoded individual structured field information
        public abstract string Abbreviation { get; }
        public abstract string Title { get; }
        protected abstract string Description { get; }
        protected abstract bool IsRepeatingGroup { get; }
        protected abstract int RepeatingGroupStart { get; }
        protected abstract List<Offset> Offsets { get; }
        protected override string StructureName => "Structured Field";

        public StructuredField(int length, string id, byte flag, int sequence) : base (length, id, 8)
        {
            Flag = flag;
            Sequence = sequence;

            Semantics = new SemanticsInfo(Title, Description, IsRepeatingGroup, RepeatingGroupStart, Offsets);
        }

        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();

            // Show description instead of title
            if (!string.IsNullOrWhiteSpace(Description))
            {
                sb.AppendLine(Description);
                sb.AppendLine();
            }

            sb.Append(GetOffsetDescriptions());

            return sb.ToString();
        }
        
        // This method can be overridden if it is complicated to parse
        protected override string GetOffsetDescriptions()
        {
            StringBuilder sb = new StringBuilder();

            if (Offsets.Count == 0)
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

            // If this is a repeating group identifier, loop through each subsection of offsets
            int sectionIncrement = 1;
            int rgStartOffset = RepeatingGroupStart;
            int skip = rgStartOffset;
            int rgLength = Data.Length;
            do
            {
                if (IsRepeatingGroup)
                {
                    sb.AppendLine($"SECTION {sectionIncrement++}");
                }

                for (int i = 0; i < Offsets.Count; i++)
                {
                    // Get the rest of the data, skipping already processed info
                    byte[] skippedData = Data.Skip(skip).ToArray();

                    // Get the length of the current repeating group (or just the length of the data if not repeating)
                    if (IsRepeatingGroup)
                    {
                        // If it is fixed, it will be the first byte of data overall. Otherwise, it will be the first byte of the current segment
                        rgLength = rgStartOffset == 0 ? skippedData[0] : Data[0];

                        // If this offset is past the current RG length, break out of the loop
                        if (Offsets[i].StartingIndex >= rgLength) break;
                    }

                    // Calculate section of bytes to take from data
                    string nextOffsetIdx = i == Offsets.Count - 1 ? "n" : Offsets[i + 1].StartingIndex.ToString();
                    int take = (nextOffsetIdx == "n" ? rgLength : int.Parse(nextOffsetIdx)) - Offsets[i].StartingIndex;
                    byte[] sectionedData = skippedData.Take(take).ToArray();

                    // Build description by datatype
                    switch (Offsets[i].DataType)
                    {
                        // For triplets, call the triplet parser
                        case Lookups.DataTypes.TRIPS:
                            if (i > 0) sb.AppendLine();
                            sb.AppendLine(Triplet.ParseAll(sectionedData));
                            break;

                        // Everything else
                        default:
                            // Skip non descriptive offsets
                            if (!string.IsNullOrWhiteSpace(Offsets[i].Description))
                            {
                                // Write out this offset's description
                                sb.AppendLine(Offsets[i].DisplayDataByType(sectionedData));
                            }
                            break;
                    }

                    // Increment the skip value so we take the correct offset of data next iteration
                    skip += take;
                }

                sb.AppendLine();

            } while (Offsets.Count > 0 && skip < Data.Length);

            return sb.ToString().Trim();
        }
    }
}
