using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AFPParser
{
    public abstract class Triplet : DataStructure
    {
        // Properties which must be implemented by individual triplets
        protected override string StructureName => "Triplet";

        public Triplet(string id, byte[] introcuder, byte[] data) : base(id, introcuder, data) { }

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
                string id = triplet[1].ToString("X2");
                byte[] introducer = new byte[2];
                byte[] data = new byte[triplet.Length - 2];
                Array.ConstrainedCopy(triplet, 0, introducer, 0, 2);
                Array.ConstrainedCopy(triplet, 2, data, 0, data.Length);
                allTrips.Add((Triplet)Activator.CreateInstance(tripletType, id, introducer, data));
            }

            // Parse all triplet data
            allTrips.ForEach(t => t.ParseData());

            return allTrips;
        }

        public override string GetFullDescription()
        {
            StringBuilder sb = new StringBuilder();

            // Use spaced class name instead of title
            sb.AppendLine($"{SpacedClassName} ({StructureName} 0x{HexID})");
            sb.Append(GetOffsetDescriptions());

            return sb.ToString();
        }

        public override void ParseData()
        {
            // TODO: Remove this if and when each triplet parses the data in their own way
        }
    }
}