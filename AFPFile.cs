using System;
using System.IO;
using System.Linq;
using AFPParser.Containers;
using System.Collections.Generic;
using AFPParser.StructuredFields;

namespace AFPParser
{
    public class AFPFile
    {
        public event Action<string> ErrorEvent;

        public IReadOnlyList<StructuredField> Fields { get; private set; }
        public IReadOnlyList<Container> PageSegments { get; private set; }
        public IReadOnlyList<FontObjectContainer> Fonts { get; private set; }

        public AFPFile()
        {
            Fields = new List<StructuredField>();
        }

        public bool LoadData(string fileName)
        {
            try
            {
                // First, read all AFP file bytes into memory
                byte[] byteList = File.ReadAllBytes(fileName);

                // Next, loop through each 5A block and store a StructuredField object
                int curIdx = 0;
                List<StructuredField> fieldList = new List<StructuredField>();
                while (curIdx < byteList.Length - 1)
                {
                    byte[] lengthBytes = new byte[2], sequenceBytes = new byte[2], identifierBytes = new byte[3];
                    string curOffsetHex = $"0x{curIdx.ToString("X8")}";

                    // Check for 0x5A prefix on each field
                    if (byteList[curIdx] != 0x5A)
                        throw new Exception($"Unexpected byte at offset {curOffsetHex}. Is it a true AFP file?");

                    // Check for expected length of data
                    if (curIdx + 9 > byteList.Length)
                        throw new Exception($"No room for SFI at offset {curOffsetHex}. Is it a true AFP file?");

                    // Read the introducer
                    Array.ConstrainedCopy(byteList, curIdx + 1, lengthBytes, 0, 2);
                    Array.ConstrainedCopy(byteList, curIdx + 3, identifierBytes, 0, 3);
                    Array.ConstrainedCopy(byteList, curIdx + 7, sequenceBytes, 0, 2);

                    // Get introducer section
                    int length = (int)DataStructure.GetNumericValue(lengthBytes, false);
                    string hex = BitConverter.ToString(identifierBytes).Replace("-", "");
                    byte flag = byteList[curIdx + 6];
                    int sequence = (int)DataStructure.GetNumericValue(sequenceBytes, false);

                    // Check the length isn't over what we can read
                    if (curIdx + 1 + length > byteList.Length)
                        throw new Exception($"Invalid field length of {length} at offset {curOffsetHex}. Is it a true AFP file?");

                    // Lookup what type of field we need by the structured fields hex dictionary
                    Type fieldType = typeof(StructuredFields.UNKNOWN);
                    if (Lookups.StructuredFields.ContainsKey(hex))
                        fieldType = Lookups.StructuredFields[hex];
                    StructuredField field = (StructuredField)Activator.CreateInstance(fieldType, length, hex, flag, sequence);

                    // Populate the data byte by byte
                    for (int i = 0; i < field.Data.Length; i++)
                    {
                        // If we read a length that isn't true (in case of a non-AFP file), make sure we don't go out of index bounds
                        int byteIndex = curIdx + 9 + i;
                        if (byteIndex >= byteList.Length) break;
                        field.Data[i] = byteList[curIdx + 9 + i];
                    }

                    // Append to AFP file
                    fieldList.Add(field);

                    // Go to next 5A
                    curIdx += field.Length + 1;
                }

                Fields = fieldList;

                // Now that all fields have been set, parse all data into their own storage methods
                ParseData();

                // Load any resources that may be in the same folder
                LoadResources();
            }
            catch (Exception ex)
            {
                ErrorEvent?.Invoke($"Error: {ex.Message}");
                return false;
            }

            return true;
        }

        private void ParseData()
        {
            // Create containers for applicable groups of fields
            List<Container> activeContainers = new List<Container>();
            foreach (StructuredField sf in Fields)
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

                // If this is an END tag, remove the last container from our active container list
                if (typeCode == "A9")
                    activeContainers.Remove(activeContainers.Last());
            }

            // Now that everything is structured, parse all data by individual handlers
            foreach (StructuredField sf in Fields)
            {
                sf.ParseData();
                
                // Parse container data after parsing all fields inside of it
                if (sf.ID.Substring(2, 2) == "A9") sf.LowestLevelContainer.ParseContainerData();
            }
        }

        private void LoadResources()
        {
            List<Container> allContainers = Fields.Select(f => f.LowestLevelContainer).Distinct().ToList();

            // Embedded IM and IOCA containers
            List<ImageObjectContainer> IOCAContainers = allContainers.OfType<ImageObjectContainer>().ToList();
            List<IMImageContainer> IMContainers = allContainers.OfType<IMImageContainer>().ToList();

            // Embedded fonts
            List<FontObjectContainer> fontContainers = allContainers.OfType<FontObjectContainer>().ToList();

            // Search in the executable's current directory
            string searchPath = Environment.CurrentDirectory;

            // Referenced page segments
            List<string> segments = Fields.OfType<IPS>().Select(f => f.SegmentName).Distinct().ToList();

            // Referenced fonts
            List<MCF1.MCF1Data> mcf1Data = Fields.OfType<MCF1>().SelectMany(f => f.MappedData).ToList();
            List<string> codedFontNames = mcf1Data.Select(m => m.CodedFontName).Distinct().ToList();
            List<string> fontCharacterSetNames = mcf1Data.Select(m => m.FontCharacterSetName).Distinct().ToList();

            // Store containers in public variables
            PageSegments = IOCAContainers.Cast<Container>().Concat(IMContainers).ToList();
            Fonts = fontContainers;
        }
    }
}
