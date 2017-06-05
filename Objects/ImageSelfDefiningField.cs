using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace AFPParser
{
    public abstract class ImageSelfDefiningField : DataStructure
    {
        // Properties which must be implemented by individual SDFs
        protected abstract string Description { get; }
        protected abstract List<Offset> Offsets { get; }    // Keep in mind that offset 0 in code is actually offset 2, since the first two bytes are always the same
        protected override string StructureName => "Image Self Defining Field";

        public ImageSelfDefiningField(int paramLength, string id, byte[] data) : base(paramLength, id, id.Length - 2)
        {
            // SDFs never have repeating groups
            Semantics = new SemanticsInfo(SpacedClassName, Description, false, 0, Offsets);

            // Set data
            for (int i = 0; i < Data.Length; i++)
                Data[i] = data[i];
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

                // Get the ID, length, and data bytes
                string sId = isExtended ? BitConverter.ToString(new[] { sdfData[i], sdfData[i + 1] }).Replace("-", "") : sdfData[i].ToString("X2");
                byte id = byte.Parse(sId.Substring(sId.Length - 2, 2), System.Globalization.NumberStyles.HexNumber);
                int length = (int)GetNumericValue(isExtended ? new[] { sdfData[i + 2], sdfData[i + 3] } : new[] { sdfData[i + 1] }, false);
                byte[] data = new byte[length];
                Array.ConstrainedCopy(sdfData, i + (isExtended ? 4 : 2), data, 0, length);

                // Create an instance of the lookup type
                Type iSDFType = typeof(ImageSelfDefiningFields.UNKNOWN);
                if (Lookups.ImageSelfDefiningFields.ContainsKey(id))
                    iSDFType = Lookups.ImageSelfDefiningFields[id];
                ImageSelfDefiningField sdf = (ImageSelfDefiningField)Activator.CreateInstance(iSDFType, length, sId, data);

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