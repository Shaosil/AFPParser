using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AFPParser
{
    public abstract class PTXControlSequence : DataStructure
    {
        public override ushort Length
        {
            get
            {
                int introducerLength = IsChained ? 2 : 4;
                return (ushort)(introducerLength + Data.Length);
            }
        }
        public bool IsChained => Introducer.Length == 2;

        // Properties which must be implemented by individual control sequences
        public abstract string Abbreviation { get; }
        protected override string StructureName => "Control Sequence";

        public PTXControlSequence(byte id, byte[] introducer, byte[] data) : base(new byte[1] { id }, introducer, data) { }

        protected override void SyncIntroducer()
        {
            Introducer[IsChained ? 0 : 2] = (byte)Length;
            Introducer[IsChained ? 1 : 3] = HexID[0];
        }

        public override string GetFullDescription()
        {
            StringBuilder sb = new StringBuilder();

            // Use description instead of title
            sb.AppendLine($"{Description} ({StructureName} 0x{HexID})");
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

            // // The introducer will have a 2B D3 prefix when unchained
            int curIndex = 0;
            bool chained = false;
            while (curIndex < csData.Length)
            {
                // If unchained, add 2 to every index since there is a prefix
                int extraIndexes = chained ? 0 : 2;

                int length = csData[curIndex + extraIndexes];
                byte csTypeByte = csData[curIndex + 1 + extraIndexes];
                chained = csTypeByte % 2 == 1; // If the function type is odd

                // Get the introducer and contents
                byte[] introducer = new byte[2 + extraIndexes];
                byte[] data = new byte[length - 2];
                Array.ConstrainedCopy(csData, curIndex, introducer, 0, 2 + extraIndexes);
                Array.ConstrainedCopy(csData, curIndex + 2 + extraIndexes, data, 0, data.Length);

                // Build and add the sequence by data type
                Type CSType = typeof(PTXControlSequences.UNKNOWN);
                if (Lookups.PTXControlSequences.ContainsKey(csTypeByte)) CSType = Lookups.PTXControlSequences[csTypeByte];
                PTXControlSequence sequence = (PTXControlSequence)Activator.CreateInstance(CSType, csTypeByte, introducer, data);
                csiList.Add(sequence);

                curIndex += length + extraIndexes;
            }

            // Parse all data
            foreach (PTXControlSequence sequence in csiList)
                sequence.ParseData();

            return csiList;
        }
    }
}