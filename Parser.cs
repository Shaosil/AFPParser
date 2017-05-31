using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace AFPParser
{
    public class Parser
    {
        public static BindingList<StructuredField> AfpFile { get; set; }
        public static List<Container> AllContainers { get; set; }

        public void LoadData(string fileName)
        {
            // First, read all AFP file bytes into memory
            byte[] byteList = File.ReadAllBytes(fileName);

            // Next, loop through each 5A block and store a StructuredField object
            int curIdx = 0;
            AfpFile = new BindingList<StructuredField>();
            while (curIdx < byteList.Length - 1)
            {
                if (byteList[curIdx] != 0x5A)
                {
                    MessageBox.Show($"Unexpected byte at offset 0x{curIdx.ToString("X8")}. Is it a true AFP file?", "AFP Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Grab the raw bytes for some of the sections
                byte[] lengthBytes = new byte[2], sequenceBytes = new byte[2], identifierBytes = new byte[3];
                Array.ConstrainedCopy(byteList, curIdx + 1, lengthBytes, 0, 2);
                Array.ConstrainedCopy(byteList, curIdx + 3, identifierBytes, 0, 3);
                Array.ConstrainedCopy(byteList, curIdx + 7, sequenceBytes, 0, 2);

                // Get introducer section
                int length = (int)DataStructure.GetNumericValue(lengthBytes, false);
                string hex = BitConverter.ToString(identifierBytes).Replace("-", "");
                byte flag = byteList[curIdx + 6];
                int sequence = (int)DataStructure.GetNumericValue(sequenceBytes, false);

                // Lookup what type of field we need by the structured fields hex dictionary
                Type fieldType = typeof(StructuredFields.UNKNOWN);
                if (Lookups.StructuredFields.ContainsKey(hex))
                    fieldType = Lookups.StructuredFields[hex];
                StructuredField field = (StructuredField)Activator.CreateInstance(fieldType, length, hex, flag, sequence);

                // Populate the data byte by byte
                for (int i = 0; i < field.Data.Length; i++)
                    field.Data[i] = byteList[curIdx + 9 + i];

                // Append to AFP file
                AfpFile.Add(field);

                // Go to next 5A
                curIdx += field.Length + 1;
            }

            // Now that all fields have been set, parse all data into their own storage methods
            ParseData();
        }

        private void ParseData()
        {
            // Create containers for applicable groups of fields
            AllContainers = new List<Container>();
            List<Container> activeContainers = new List<Container>();
            foreach (StructuredField sf in AfpFile)
            {
                string typeCode = sf.ID.Substring(2, 2);

                // If this is a BEGIN tag, create a new container and add it to the list, and set it as active
                if (typeCode == "A8")
                {
                    Container c = new Container();
                    AllContainers.Add(c);
                    activeContainers.Add(c);
                }

                // Add this field to each active container's list of fields
                foreach (Container c in activeContainers)
                    c.Fields.Add(sf);

                // Set the lowest level container if there are any
                if (activeContainers.Any())
                    sf.LowestLevelContainer = activeContainers.Last();

                // If this is an END tag, remove the last container from our active container list
                if (typeCode == "A9")
                    activeContainers.RemoveAt(activeContainers.Count - 1);
            }

            // Now that everything is structured, parse all data by individual handlers
            foreach (StructuredField sf in AfpFile)
                sf.ParseData();
        }
    }
}
