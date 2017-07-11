using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFPParser
{
    public abstract class ImageSelfDefiningField : DataStructure
    {
        // SDFs do not include the introducer length in the lengh property
        public override ushort Length => (ushort)Data.Length;

        // Properties which must be implemented by individual SDFs
        protected override string StructureName => "Image Self Defining Field";

        public ImageSelfDefiningField(byte[] id, byte[] data) : base(id, data) { }

        protected override void SyncIntroducer()
        {
            int extraBytes = HexID.Length > 1 ? 2 : 0;
            if (Introducer == null) Introducer = new byte[2 + extraBytes];

            // Either single or double byte fields in the introducer
            if (Introducer.Length == 2)
            {
                Introducer[0] = HexID[0];
                Introducer[1] = (byte)Length;
            }
            else
            {
                byte[] len = BitConverter.GetBytes(Length);
                if (BitConverter.IsLittleEndian) len = len.Reverse().ToArray();

                Array.ConstrainedCopy(HexID, 0, Introducer, 0, 2);
                Array.ConstrainedCopy(len, 0, Introducer, 2, 2);
            }
        }

        public static List<ImageSelfDefiningField> GetAllSDFs(byte[] sdfData)
        {
            List<ImageSelfDefiningField> sdfList = new List<ImageSelfDefiningField>();

            // Container info
            byte[] beginCodes = new byte[4] { 0x70, 0x8C, 0x8E, 0x91 };
            byte[] endCodes = new byte[4] { 0x71, 0x8D, 0x8F, 0x93 };
            List<Container> activeContainers = new List<Container>();

            for (int i = 0; i < sdfData.Length;)
            {
                // Check the first byte of the code. If it's 0xFE, this is an extended format SDF and will have two byte codes and lengths instead of one
                bool isExtended = sdfData[i] == 0xFE;

                // Get the ID, length, introducer, and data bytes
                byte[] idArray = isExtended ? new byte[2] { 0xFE, sdfData[i + 1] } : new byte[1] { sdfData[i] };
                byte id = idArray.Last();
                int length = isExtended ? GetNumericValue<ushort>(new[] { sdfData[i + 2], sdfData[i + 3] })
                    : GetNumericValue<byte>(new[] { sdfData[i + 1] });
                byte[] data = new byte[length];
                Array.ConstrainedCopy(sdfData, i + (isExtended ? 4 : 2), data, 0, length);

                // Create an instance of the lookup type
                Type iSDFType = typeof(ImageSelfDefiningFields.UNKNOWN);
                if (Lookups.ImageSelfDefiningFields.ContainsKey(id)) iSDFType = Lookups.ImageSelfDefiningFields[id];
                ImageSelfDefiningField sdf = (ImageSelfDefiningField)Activator.CreateInstance(iSDFType, idArray, data);

                // If this is a begin tag, add a new container to our active list
                if (beginCodes.Contains(id))
                    activeContainers.Add(sdf.NewContainer);

                // Set lowest level container, if any
                if (activeContainers.Any())
                    sdf.LowestLevelContainer = activeContainers.Last();

                // Add this field to all containers
                foreach (Container c in activeContainers)
                    c.Structures.Add(sdf);

                // If this is the last structure in a container, close it up and parse any specific container data
                if (endCodes.Contains(id))
                {
                    Container c = activeContainers.Last();
                    activeContainers.Remove(c);
                    c.ParseContainerData();
                }

                sdfList.Add(sdf);

                // Go to next block
                i += (isExtended ? 4 : 2) + length;
            }

            return sdfList;
        }

        public static string GetAllDescriptions(byte[] sdfData)
        {
            StringBuilder sb = new StringBuilder();

            foreach (ImageSelfDefiningField sdf in GetAllSDFs(sdfData))
                sb.AppendLine(sdf.GetFullDescription());

            return sb.ToString();
        }

        public override void ParseData()
        {
            // TODO: Remove this if and when each SDF parses the data in their own way
        }
    }
}