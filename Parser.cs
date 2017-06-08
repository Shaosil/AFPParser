using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using AFPParser.StructuredFields;

namespace AFPParser
{
    public class Parser
    {
        public event Action<string> ErrorEvent;

        public List<StructuredField> StructuredFields { get; set; }

        public void LoadData(string fileName)
        {
            try
            {
                // First, read all AFP file bytes into memory
                byte[] byteList = File.ReadAllBytes(fileName);

                // Next, loop through each 5A block and store a StructuredField object
                int curIdx = 0;
                StructuredFields = new List<StructuredField>();
                while (curIdx < byteList.Length - 1)
                {
                    if (byteList[curIdx] != 0x5A)
                    {
                        ErrorEvent?.Invoke($"Unexpected byte at offset 0x{curIdx.ToString("X8")}. Is it a true AFP file?");
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
                    StructuredFields.Add(field);

                    // Go to next 5A
                    curIdx += field.Length + 1;
                }

                // Now that all fields have been set, parse all data into their own storage methods
                ParseData();
            }
            catch (Exception ex)
            {
                StructuredFields = new List<StructuredField>();
                ErrorEvent?.Invoke($"Error: {ex.Message}");
            }
        }

        private void ParseData()
        {
            // Create containers for applicable groups of fields
            List<Container> activeContainers = new List<Container>();
            foreach (StructuredField sf in StructuredFields)
            {
                string typeCode = sf.ID.Substring(2, 2);

                // If this is a BEGIN tag, create a new container and add it to the list, and set it as active
                if (typeCode == "A8")
                    activeContainers.Add(sf.NewContainer);

                // Add this field to each active container's list of fields
                foreach (Container c in activeContainers)
                    c.Structures.Add(sf);

                // Set the lowest level container if there are any
                if (activeContainers.Any())
                    sf.LowestLevelContainer = activeContainers.Last();

                // If this is an END tag, remove the last container from our active container list, and parse any group data for the collection of fields
                if (typeCode == "A9")
                {
                    Container mostRecentContainer = activeContainers.Last();
                    activeContainers.Remove(mostRecentContainer);
                    mostRecentContainer.ParseContainerData();
                }
            }

            // Now that everything is structured, parse all data by individual handlers
            foreach (StructuredField sf in StructuredFields)
                sf.ParseData();
        }
    }
}
