using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFPParser
{
    public abstract class Triplet : DataStructure
    {
        // Properties which must be implemented by individual triplets
        protected override string StructureName => "Triplet";

        public Triplet(byte id, byte[] data) : base(new byte[1] { id }, data) { }

        public static List<Triplet> GetAllTriplets(byte[] tripletData)
        {
            List<Triplet> allTrips = new List<Triplet>();

            // Find each triplet segment by reading the length
            List<byte[]> tripletBytes = new List<byte[]>();
            int curIndex = 0;
            while (curIndex < tripletData.Length)
            {
                // The first byte is always length, so use that to add each triplet section to the list of byte arrays
                int length = tripletData[curIndex];
                tripletBytes.Add(tripletData.Skip(curIndex).Take(length).ToArray());
                curIndex += length;
            }

            // Create instances of matching triplet objects by IDs
            foreach (byte[] triplet in tripletBytes)
            {
                // Get the type of triplet, create the object, and add it to the list
                Type tripletType = typeof(Triplets.UNKNOWN);
                if (Lookups.Triplets.ContainsKey(triplet[1])) tripletType = Lookups.Triplets[triplet[1]];

                // Get data for constructor
                byte id = triplet[1];
                byte[] data = new byte[triplet.Length - 2];
                Array.ConstrainedCopy(triplet, 2, data, 0, data.Length);
                allTrips.Add((Triplet)Activator.CreateInstance(tripletType, id, data));
            }

            // Parse all triplet data
            allTrips.ForEach(t => t.ParseData());

            return allTrips;
        }

        public override string GetFullDescription()
        {
            StringBuilder sb = new StringBuilder();

            // Use spaced class name instead of title
            sb.AppendLine($"{SpacedClassName} ({StructureName} 0x{HexIDStr})");
            sb.Append(GetOffsetDescriptions());

            return sb.ToString();
        }

        public override void ParseData()
        {
            // TODO: Remove this if and when each triplet parses the data in their own way
        }

        protected override void SyncIntroducer()
        {
            if (Introducer == null) Introducer = new byte[2];

            Introducer[0] = (byte)Length;
            Introducer[1] = HexID[0];
        }
    }
}