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
        private byte _flag;
        private ushort _sequence;

        // Properties directly converted from raw hex data
        public byte Flag
        {
            get { return _flag; }
            set
            {
                _flag = value;
                SyncIntroducer();
            }
        }
        public ushort Sequence
        {
            get { return _sequence; }
            set
            {
                _sequence = value;
                SyncIntroducer();
            }
        }

        // Abstract properties derived from hard coded individual structured field information
        public abstract string Abbreviation { get; }
        protected abstract bool IsRepeatingGroup { get; }
        protected abstract int RepeatingGroupStart { get; }
        protected virtual int RepeatingGroupLength { get { return Data.Length; } }
        protected override string StructureName => "Structured Field";

        // Parsed Data
        public IReadOnlyList<Triplet> Triplets { get; private set; }

        public StructuredField(byte[] id, byte[] introducer, byte[] data) : base(id, introducer, data)
        {
            Flag = introducer[5];
            Sequence = (ushort)GetNumericValue(new byte[2] { introducer[6], introducer[7] }, false);
            Triplets = new List<Triplet>();
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

        protected override void SyncIntroducer()
        {
            byte[] len = BitConverter.GetBytes(Length);
            byte[] seq = BitConverter.GetBytes(Sequence);
            if (BitConverter.IsLittleEndian)
            {
                len = len.Reverse().ToArray();
                seq = seq.Reverse().ToArray();
            }

            Array.ConstrainedCopy(len, 0, Introducer, 0, 2);
            Array.ConstrainedCopy(HexID, 0, Introducer, 2, 3);
            Introducer[5] = Flag;
            Array.ConstrainedCopy(seq, 0, Introducer, 6, 2);
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
            int skip = IsRepeatingGroup ? RepeatingGroupStart : Offsets[0].StartingIndex;
            int sectionLength = RepeatingGroupLength;
            do
            {
                for (int i = 0; i < Offsets.Count; i++)
                {
                    // If we are past the data length, break out of the loop
                    if (skip > Data.Length) break;

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

                        // For special sub sectioned data types, call the static parser
                        case Lookups.DataTypes.IMAGESDFS:
                            if (i > 0) sb.AppendLine();
                            sb.AppendLine(ImageSelfDefiningField.GetAllDescriptions(sectionedData));
                            break;

                        // For Triplets, parse them all at once
                        case Lookups.DataTypes.TRIPS:
                            sb.AppendLine();
                            foreach (Triplet t in Triplets)
                                sb.AppendLine(t.GetFullDescription());
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
            // Parse triplets automatically if they exist
            Offset tripsOffset = Offsets.FirstOrDefault(o => o.DataType == Lookups.DataTypes.TRIPS);
            if (tripsOffset != null)
                Triplets = Triplet.GetAllTriplets(Data.Skip(tripsOffset.StartingIndex).ToArray());
        }

        public T GetTriplet<T>() where T : Triplet
        {
            return Triplets.OfType<T>().FirstOrDefault();
        }

        public List<T> GetTriplets<T>() where T : Triplet
        {
            return Triplets.OfType<T>().ToList();
        }
    }
}