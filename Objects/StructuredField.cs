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

        // Abstract properties derived from hard coded individual structured field information
        public abstract string Abbreviation { get; }
        public abstract string Title { get; }
        protected abstract string Description { get; }
        protected abstract bool IsRepeatingGroup { get; }
        protected abstract int RepeatingGroupStart { get; }
        protected virtual int RepeatingGroupLength { get { return Data.Length; } }
        protected abstract List<Offset> Offsets { get; }
        protected override string StructureName => "Structured Field";

        // Container info
        public Container LowestLevelContainer { get; set; }

        public StructuredField(int length, string id, byte flag, int sequence) : base(length, id, 8)
        {
            Flag = flag;
            Sequence = sequence;
            Semantics = new SemanticsInfo(Title, Description, IsRepeatingGroup, RepeatingGroupStart, Offsets);
        }

        public override string GetFullDescription()
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
            int skip = IsRepeatingGroup ? RepeatingGroupStart : 0;
            int sectionLength = RepeatingGroupLength;
            do
            {
                for (int i = 0; i < Offsets.Count; i++)
                {
                    // If we are past the data length, break out of the loop
                    if (skip >= Data.Length) break;

                    // Get the length of the current repeating group if it is retrieved in each section (if there is not a fixed length)
                    if (IsRepeatingGroup && RepeatingGroupLength == 0 && i == 0)
                    {
                        int rgOffsetLength = Offsets[i + 1].StartingIndex;
                        sectionLength = (int)GetNumericValue(GetSectionedData(skip, rgOffsetLength), false);
                    }

                    // Calculate section of bytes to take from data
                    int nextOffsetIdx = i == Offsets.Count - 1 ? 0 : Offsets[i + 1].StartingIndex;
                    int take = (nextOffsetIdx == 0 ? sectionLength : nextOffsetIdx) - Offsets[i].StartingIndex;
                    byte[] sectionedData = GetSectionedData(skip, take);

                    // Build description by data type
                    switch (Offsets[i].DataType)
                    {
                        // For triplets, call the triplet parser
                        case Lookups.DataTypes.TRIPS:
                            if (i > 0) sb.AppendLine();
                            sb.AppendLine(Triplet.ParseAll(sectionedData));
                            break;

                        // Everything else
                        default:
                            sb.Append(GetSingleOffsetDescription(Offsets[i], sectionedData));
                            break;
                    }

                    // Increment the skip value so we take the correct offset of data next iteration
                    skip += take;
                }

                sb.AppendLine();

            } while (skip < Data.Length);

            return sb.ToString().Trim();
        }

        public override void ParseData()
        {
             // TODO: Remove this if and when each structured field parses the data in their own way
        }
    }
}