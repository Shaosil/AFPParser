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
                Resources = new List<Resource>();
                Fields = LoadFields(path);

                // Store embedded resources
                List<Resource> embeddedResources = new List<Resource>();
                List<Container> allContainers = Fields.Where(f => f.LowestLevelContainer != null).Select(f => f.LowestLevelContainer).Distinct().ToList();

                // Gather embedded IM, IOCA, and Font containers
                IEnumerable<Container> IOCAContainers = allContainers.OfType<IOCAImageContainer>();
                IEnumerable<Container> IMContainers = allContainers.OfType<IMImageContainer>();
                IEnumerable<Container> fontContainers = allContainers.OfType<FontObjectContainer>();
                IEnumerable<Container> codedFontContainers = allContainers.Where(c => c.Structures[0].GetType() == typeof(BCF));

                // Add all embedded resources to the resource list
                foreach (Container c in IOCAContainers.Concat(IMContainers).Concat(fontContainers).Concat(codedFontContainers))
                {
                    // Each container has a property called "ObjectName" that can be used to get the resource name here
                    string resName = c.Structures[0].GetType().GetProperty("ObjectName").GetValue(c.Structures[0]).ToString();
                    Resource newResource = new Resource(resName, GetResourceTypeByContainer(c), true);
                    newResource.Fields = c.Structures.Cast<StructuredField>().ToList();
                    embeddedResources.Add(newResource);
                }

                // Add embedded and external referenced resources
                Resources = embeddedResources; // Do this first as to avoid duplicates
                List<Resource> referencedResources = LoadResources(Fields);

                // Try to find all referenced files
                ScanDirectoriesForResources(referencedResources);

                // Reassign the result of both
                Resources = Resources.Concat(referencedResources).ToList();
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

        private List<Resource> LoadResources(IEnumerable<StructuredField> fileFields)
        {
            List<Resource> allResources = new List<Resource>();

            // Add referenced page segments
            foreach (string s in fileFields.OfType<IPS>().Select(f => f.SegmentName).Distinct())
                allResources.Add(new Resource(s, Resource.eResourceType.PageSegment));

            // Helper lists
            List<MCF1.MCF1Data> mcf1Data = fileFields.OfType<MCF1>().SelectMany(f => f.MappedData).ToList();
            List<string> existingCodedFonts = Resources.GetNamesOfType(Resource.eResourceType.CodedFont);
            List<string> existingFontCharacterSets = Resources.GetNamesOfType(Resource.eResourceType.FontCharacterSet);

            // Get MCF1 coded fonts that are not already embedded
            List<string> codedFonts = mcf1Data.Where(m => !string.IsNullOrWhiteSpace(m.CodedFontName)).Select(m => m.CodedFontName)
                .Except(existingCodedFonts).ToList();

            // Get MCF1 font character sets that are not already embedded
            List<string> fontCharacterSets = mcf1Data.Where(m => !string.IsNullOrWhiteSpace(m.FontCharacterSetName)).Select(m => m.FontCharacterSetName)
                .Except(existingFontCharacterSets).ToList();

            // Get Coded Fonts font character sets that are not already embedded
            fontCharacterSets.AddRange(fileFields.OfType<CFI>().SelectMany(f => f.FontInfoList.Select(i => i.FontCharacterSetName)).Except(existingCodedFonts));

            // Add referenced fonts
            foreach (string s in codedFonts)
                allResources.Add(new Resource(s, Resource.eResourceType.CodedFont));
            foreach (string s in fontCharacterSets)
                allResources.Add(new Resource(s, Resource.eResourceType.FontCharacterSet));

            return allResources;
        }

        public void ScanDirectoriesForResources(IEnumerable<Resource> resources)
        {
            // Attempt to find a matching file for each resource and load it
            List<FileInfo> allFiles = ResourceDirectories.SelectMany(d => new DirectoryInfo(d).GetFiles()).ToList();
            foreach (Resource r in resources)
            {
                FileInfo matchingFile = allFiles.FirstOrDefault(f => f.Name.ToUpper().Trim() == r.ResourceName);
                if (matchingFile != null)
                {
                    r.Fields = LoadFields(matchingFile.FullName);

                    // Also see if there are any additional resources to load from within those fields
                    List<Resource> extraResources = LoadResources(r.Fields);

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
            else if (cType == typeof(IOCAImageContainer)) rType = Resource.eResourceType.IOCAImage;
            else if (cType == typeof(FontObjectContainer)) rType = Resource.eResourceType.FontCharacterSet;
            else if (c.Structures[0].GetType() == typeof(BCF)) rType = Resource.eResourceType.CodedFont;
            else if (c.Structures[0].GetType() == typeof(IPS)) rType = Resource.eResourceType.PageSegment;

            return rType;
        }

        public class Resource
        {
            public enum eResourceType { Unknown, IMImage, IOCAImage, CodedFont, FontCharacterSet, PageSegment }

            public IReadOnlyList<StructuredField> Fields { get; set; }
            public string ResourceName { get; private set; }
            public eResourceType ResourceType { get; private set; }
            public bool IsLoaded => Fields.Any();
            public bool IsEmbedded { get; private set; }
            public string Message => IsEmbedded ? "Embedded" : IsLoaded ? "Loaded" : "File not found";

            public Resource(string fName, eResourceType rType, bool embedded = false)
            {
                Fields = new List<StructuredField>();
                ResourceName = fName.ToUpper().Trim();
                ResourceType = rType;
                IsEmbedded = embedded;
            }
        }
    }
}