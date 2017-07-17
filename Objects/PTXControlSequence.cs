using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFPParser
{
    public abstract class PTXControlSequence : DataStructure
    {
        public static readonly byte[] Prefix = new byte[2] { 0x2B, 0xD3 };
        public bool IsChained => Introducer.Length == 2;

        // Properties which must be implemented by individual control sequences
        public abstract string Abbreviation { get; }
        protected override string StructureName => "Control Sequence";

        public PTXControlSequence(byte id, bool hasPrefix, byte[] data) : base(new byte[1] { id }, data)
        {
            // Insert the prefix if necessary
            if (hasPrefix) Introducer = Prefix.Concat(Introducer).ToArray();
        }

        protected override void SyncIntroducer()
        {
            if (Introducer == null) Introducer = new byte[2];

            // Only use two bytes since we'll be here from the base constructor before we know whether or not to add the prefix
            Introducer[0] = (byte)Length;
            Introducer[1] = HexID[0];
        }

        public override string GetFullDescription()
        {
            StringBuilder sb = new StringBuilder();

            // Use description instead of title
            sb.AppendLine($"{Description} ({StructureName} 0x{HexIDStr})");
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
            bool hasPrefix = true;
            while (curIndex < csData.Length)
            {
                // If unchained, add 2 to every index since there is a prefix
                int extraIndexes = hasPrefix ? 2 : 0;

                int length = csData[curIndex + extraIndexes];
                byte csTypeByte = csData[curIndex + 1 + extraIndexes];

                // Get the introducer and contents
                byte[] data = new byte[length - 2];
                Array.ConstrainedCopy(csData, curIndex + 2 + extraIndexes, data, 0, data.Length);

                // Build and add the sequence by data type
                Type CSType = typeof(PTXControlSequences.UNKNOWN);
                if (Lookups.PTXControlSequences.ContainsKey(csTypeByte)) CSType = Lookups.PTXControlSequences[csTypeByte];
                PTXControlSequence sequence = (PTXControlSequence)Activator.CreateInstance(CSType, csTypeByte, hasPrefix, data);
                csiList.Add(sequence);

                curIndex += length + extraIndexes;

                // Set the prefix for the next sequence, dependant on chained status (chained if function type is odd)
                hasPrefix = csTypeByte % 2 != 1;
            }

            // Parse all data
            foreach (PTXControlSequence sequence in csiList)
                sequence.ParseData();

            return csiList;
        }
    }
}