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
        public List<string> ResourceDirectories { get; private set; }
        public IReadOnlyList<Resource> Resources { get; private set; }
        public IReadOnlyDictionary<Resource, List<Container>> ResourceUses { get; private set; }

        public AFPFile()
        {
            Fields = new List<StructuredField>();
            ResourceDirectories = new List<string>() { Environment.CurrentDirectory };
            Resources = new List<Resource>();
        }

        public bool LoadData(string path)
        {
            try
            {
                // Load all data into main fields property
                Fields = LoadFields(path);

                // Store embedded resources
                List<Resource> allResources = new List<Resource>();
                List<Container> allContainers = Fields.Select(f => f.LowestLevelContainer).Distinct().ToList();

                // Gather embedded IM, IOCA, and Font containers
                IEnumerable<Container> IOCAContainers = allContainers.OfType<ImageObjectContainer>();
                IEnumerable<Container> IMContainers = allContainers.OfType<IMImageContainer>();
                IEnumerable<Container> fontContainers = allContainers.OfType<FontObjectContainer>();

                // Add all embedded resources to the resource list
                foreach (Container c in IOCAContainers.Concat(IMContainers).Concat(fontContainers))
                {
                    Resource newResource = new Resource("Embedded", GetResourceTypeByContainer(c));
                    newResource.Fields = c.Structures.Cast<StructuredField>().ToList();
                    allResources.Add(newResource);
                }

                // Add embedded and external referenced resources
                Resources = allResources.Concat(LoadResources(Fields.ToList())).ToList();

                // Try to find all referenced files
                ScanDirectoriesForResources(allResources);
            }
            catch (Exception ex)
            {
                ErrorEvent?.Invoke($"Error: {ex.Message}");
                return false;
            }

            return true;
        }

        private List<StructuredField> LoadFields(string path)
        {
            List<StructuredField> fields = new List<StructuredField>();

            // First, read all AFP file bytes into memory
            byte[] byteList = File.ReadAllBytes(path);

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
                fields.Add(field);

                // Go to next 5A
                curIdx += field.Length + 1;
            }

            // Now that all fields have been set, parse all data into their own storage methods
            ParseData(fields);

            return fields;
        }

        private void ParseData(List<StructuredField> fields)
        {
            // Create containers for applicable groups of fields
            List<Container> activeContainers = new List<Container>();
            foreach (StructuredField sf in fields)
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
            foreach (StructuredField sf in fields)
            {
                sf.ParseData();
                
                // Parse container data after parsing all fields inside of it
                if (sf.ID.Substring(2, 2) == "A9") sf.LowestLevelContainer.ParseContainerData();
            }
        }

        private List<Resource> LoadResources(List<StructuredField> fields)
        {
            List<Resource> allResources = new List<Resource>();

            // Add referenced page segments
            foreach (string s in fields.OfType<IPS>().Select(f => f.SegmentName).Distinct())
                allResources.Add(new Resource(s, Resource.eResourceType.PageSegment));

            // Add referenced MCF1 coded fonts
            List<MCF1.MCF1Data> mcf1Data = fields.OfType<MCF1>().SelectMany(f => f.MappedData).ToList();
            foreach (string s in mcf1Data.Where(m => !string.IsNullOrWhiteSpace(m.CodedFontName)).Select(m => m.CodedFontName).Distinct())
                allResources.Add(new Resource(s, Resource.eResourceType.CodedFont));

            // Add referenced MCF1 font character sets
            foreach (string s in mcf1Data.Where(m => !string.IsNullOrWhiteSpace(m.FontCharacterSetName)).Select(m => m.FontCharacterSetName).Distinct())
                allResources.Add(new Resource(s, Resource.eResourceType.FontCharacterSet));

            // Add referenced Coded Font's font character sets from CFI fields
            foreach (string s in fields.OfType<CFI>().SelectMany(f => f.FontInfoList.Select(i => i.FontCharacterSetName)))
                allResources.Add(new Resource(s, Resource.eResourceType.FontCharacterSet));

            return allResources;
        }

        public void ScanDirectoriesForResources(IEnumerable<Resource> resources)
        {
            // Attempt to find a matching file for each resource and load it
            List<FileInfo> allFiles = ResourceDirectories.SelectMany(d => new DirectoryInfo(d).GetFiles()).ToList();
            foreach (Resource r in resources)
            {
                FileInfo matchingFile = allFiles.FirstOrDefault(f => f.Name.ToUpper().Trim() == r.FileName);
                if (matchingFile != null)
                {
                    r.Fields = LoadFields(matchingFile.FullName);

                    // Also see if there are any additional resources to load from within those fields
                    List<Resource> extraResources = LoadResources(r.Fields.ToList());

                    // If there are, add them to the total resources list and recursively search for files within known directories
                    if (extraResources.Any())
                    {
                        Resources = Resources.Concat(extraResources).ToList();
                        ScanDirectoriesForResources(extraResources);
                    }
                }
            }
        }

        private Resource.eResourceType GetResourceTypeByContainer(Container c)
        {
            Resource.eResourceType rType = Resource.eResourceType.Unknown;

            Type cType = c.GetType();
            if (cType == typeof(IMImageContainer)) rType = Resource.eResourceType.IMImage;
            else if (cType == typeof(ImageObjectContainer)) rType = Resource.eResourceType.IOCAImage;
            else if (cType == typeof(FontObjectContainer)) rType = Resource.eResourceType.FontCharacterSet;
            else if (c.DirectStructures[0].GetType() == typeof(IPS)) rType = Resource.eResourceType.PageSegment;

            return rType;
        }

        public class Resource
        {
            public enum eResourceType { Unknown, IMImage, IOCAImage, CodedFont, FontCharacterSet, PageSegment }

            public IReadOnlyList<StructuredField> Fields { get; set; }
            public string FileName { get; private set; }
            public eResourceType ResourceType { get; private set; }
            public string Message => Fields.Any() ? "Loaded" : "File not found";

            public Resource(string fName, eResourceType rType)
            {
                FileName = fName.ToUpper().Trim();
                ResourceType = rType;
                Fields = new List<StructuredField>();
            }
        }
    }
}