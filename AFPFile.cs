using System;
using System.IO;
using System.Linq;
using System.Text;
using AFPParser.Containers;
using System.Collections.Generic;
using AFPParser.StructuredFields;

namespace AFPParser
{
    public class AFPFile
    {
        private List<StructuredField> _fields;

        public event Action<string> ErrorEvent;
        public IReadOnlyList<StructuredField> Fields => _fields;
        public List<string> ResourceDirectories { get; set; }
        public IReadOnlyList<Resource> Resources { get; private set; }

        public AFPFile()
        {
            _fields = new List<StructuredField>();
            ResourceDirectories = new List<string>() { Environment.CurrentDirectory };
            Resources = new List<Resource>();
        }

        public bool LoadData(string path, bool parseData)
        {
            try
            {
                // Load all data into main fields property
                _fields = LoadFields(path, parseData);

                // Only load resources if we have parsed the data
                if (parseData)
                {
                    // Store embedded resources
                    Resources = new List<Resource>();
                    List<Resource> embeddedResources = new List<Resource>();
                    List<Container> allContainers = Fields.Where(f => f.LowestLevelContainer != null).Select(f => f.LowestLevelContainer).Distinct().ToList();

                    // Gather embedded IM, IOCA, and Font containers
                    IEnumerable<Container> IOCAContainers = allContainers.OfType<IOCAImageContainer>();
                    IEnumerable<Container> IMContainers = allContainers.OfType<IMImageContainer>();
                    IEnumerable<Container> fontContainers = allContainers.OfType<FontObjectContainer>();
                    IEnumerable<Container> codedFontContainers = allContainers.Where(c => c.Structures[0].GetType() == typeof(BCF));
                    IEnumerable<Container> codePageContainers = allContainers.Where(c => c.Structures[0].GetType() == typeof(BCP));

                    // Add all embedded resources to the resource list
                    foreach (Container c in IOCAContainers.Concat(IMContainers).Concat(fontContainers).Concat(codedFontContainers).Concat(codePageContainers))
                    {
                        // Each container has a property called "ObjectName" that can be used to get the resource name here
                        string resName = c.Structures[0].GetType().GetProperty("ObjectName").GetValue(c.Structures[0]).ToString();
                        Resource newResource = new Resource(resName, GetResourceTypeByContainer(c), true);
                        newResource.Fields = c.Structures.Cast<StructuredField>().ToList();
                        embeddedResources.Add(newResource);
                    }

                    // Add embedded and external referenced resources
                    Resources = embeddedResources; // Do this first as to avoid duplicates
                    List<Resource> referencedResources = LoadReferencedResources(Fields);

                    // Try to find all referenced files
                    ScanDirectoriesForResources(referencedResources);

                    // Combine and assign to readonly property
                    Resources = Resources.Concat(referencedResources).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorEvent?.Invoke($"Error: {ex.Message}");
                return false;
            }

            return true;
        }

        private List<StructuredField> LoadFields(string path, bool parseData)
        {
            List<StructuredField> fields = new List<StructuredField>();

            // First, read all AFP file bytes into memory
            byte[] byteList = File.ReadAllBytes(path);

            // Next, loop through each 5A block and store a StructuredField object
            int curIdx = 0;
            while (curIdx < byteList.Length - 1)
            {
                byte[] lengthBytes = new byte[2], identifierBytes = new byte[3], introducer = new byte[8], sequenceBytes = new byte[2];
                byte flag;
                string curOffsetHex = $"0x{curIdx.ToString("X8")}";

                // Check for 0x5A prefix on each field
                if (byteList[curIdx] != 0x5A)
                    throw new Exception($"Unexpected byte at offset {curOffsetHex}. Is it a true AFP file?");

                // Check for expected length of data
                if (curIdx + 9 > byteList.Length)
                    throw new Exception($"No room for SFI at offset {curOffsetHex}. Is it a true AFP file?");

                // Read the introducer
                Array.ConstrainedCopy(byteList, curIdx + 1, introducer, 0, 8);
                Array.ConstrainedCopy(introducer, 0, lengthBytes, 0, 2);
                Array.ConstrainedCopy(introducer, 2, identifierBytes, 0, 3);
                flag = introducer[5];
                Array.ConstrainedCopy(introducer, 6, sequenceBytes, 0, 2);

                // Get a couple pieces of data from introducer
                int length = (int)DataStructure.GetNumericValue(lengthBytes, false);
                ushort sequence = (ushort)DataStructure.GetNumericValue(sequenceBytes, false);

                // Check the length isn't over what we can read
                if (curIdx + 1 + length > byteList.Length)
                    throw new Exception($"Invalid field length of {length} at offset {curOffsetHex}. Is it a true AFP file?");

                // Get the data
                byte[] data = new byte[length - 8];
                Array.ConstrainedCopy(byteList, curIdx + 9, data, 0, length - 8);

                // Lookup what type of field we need by the structured fields hex dictionary
                Type fieldType = typeof(UNKNOWN);
                string idStr = BitConverter.ToString(identifierBytes).Replace("-", "");
                if (Lookups.StructuredFields.ContainsKey(idStr)) fieldType = Lookups.StructuredFields[idStr];
                StructuredField field = (StructuredField)Activator.CreateInstance(fieldType, identifierBytes, flag, sequence, data);

                // Append to AFP file
                fields.Add(field);

                // Go to next 5A
                curIdx += length + 1;
            }

            SetupContainers(fields);

            // Now that all fields have been set, parse all data into their own storage methods
            if (parseData) ParseData(fields);

            return fields;
        }

        private void SetupContainers(List<StructuredField> fields)
        {
            // Create containers for applicable groups of fields
            List<Container> activeContainers = new List<Container>();
            foreach (StructuredField sf in fields)
            {
                // If this is a BEGIN tag, create a new container and add it to the list, and set it as active
                if (sf.HexID[1] == 0xA8)
                    activeContainers.Add(sf.NewContainer);

                // Add this field to each active container's list of fields
                foreach (Container c in activeContainers)
                    c.Structures.Add(sf);

                // Set the lowest level container if there are any
                if (activeContainers.Any())
                    sf.LowestLevelContainer = activeContainers.Last();

                // If this is an END tag, remove the last container from our active container list
                if (sf.HexID[1] == 0xA9)
                    activeContainers.Remove(activeContainers.Last());
            }
        }

        private void ParseData(List<StructuredField> fields)
        {
            foreach (StructuredField sf in fields)
            {
                sf.ParseData();

                // Parse container data after parsing all fields inside of it
                if (sf.HexID[1] == 0xA9) sf.LowestLevelContainer.ParseContainerData();
            }
        }

        private List<Resource> LoadReferencedResources(IEnumerable<StructuredField> fileFields)
        {
            List<Resource> allResources = new List<Resource>();

            // Add referenced page segments
            foreach (string s in fileFields.OfType<IPS>().Where(f => f.SegmentName != null).Select(f => f.SegmentName).Distinct())
                allResources.Add(new Resource(s, Resource.eResourceType.PageSegment));

            // Helper lists
            List<MCF1.MCF1Data> mcf1Data = fileFields.OfType<MCF1>().SelectMany(f => f.MappedData).ToList();
            List<string> existingCodePages = Resources.GetNamesOfType(Resource.eResourceType.CodePage);
            List<string> existingCodedFonts = Resources.GetNamesOfType(Resource.eResourceType.CodedFont);
            List<string> existingFontCharacterSets = Resources.GetNamesOfType(Resource.eResourceType.FontCharacterSet);

            // Get MCF1 code pages that are not already embedded
            List<string> codePages = mcf1Data.Where(m => !string.IsNullOrWhiteSpace(m.CodePageName)).Select(m => m.CodePageName)
                .Except(existingCodePages).ToList();

            // Get MCF1 coded fonts that are not already embedded
            List<string> codedFonts = mcf1Data.Where(m => !string.IsNullOrWhiteSpace(m.CodedFontName)).Select(m => m.CodedFontName)
                .Except(existingCodedFonts).ToList();

            // Get MCF1 font character sets that are not already embedded
            List<string> fontCharacterSets = mcf1Data.Where(m => !string.IsNullOrWhiteSpace(m.FontCharacterSetName)).Select(m => m.FontCharacterSetName)
                .Except(existingFontCharacterSets).ToList();

            // Get Coded Fonts code pages that are not already embedded
            codePages.AddRange(fileFields.OfType<CFI>().SelectMany(f => f.FontInfoList.Select(i => i.CodePageName)).Except(existingCodePages));

            // Get Coded Fonts font character sets that are not already embedded
            fontCharacterSets.AddRange(fileFields.OfType<CFI>().SelectMany(f => f.FontInfoList.Select(i => i.FontCharacterSetName)).Except(existingCodedFonts));

            // Add referenced font infos
            foreach (string s in codePages)
                allResources.Add(new Resource(s, Resource.eResourceType.CodePage));
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
                    r.Fields = LoadFields(matchingFile.FullName, true);

                    // Also see if there are any additional resources to load from within those fields
                    List<Resource> extraResources = LoadReferencedResources(r.Fields);

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
            else if (c.Structures[0].GetType() == typeof(BCP)) rType = Resource.eResourceType.CodePage;
            else if (c.Structures[0].GetType() == typeof(BCF)) rType = Resource.eResourceType.CodedFont;
            else if (c.Structures[0].GetType() == typeof(IPS)) rType = Resource.eResourceType.PageSegment;

            return rType;
        }

        public void AddField(StructuredField field, int index = -1)
        {
            // Insert at specified index if needed. Else, append
            if (index >= 0)
                _fields.Insert(index, field);
            else
                _fields.Add(field);

            // Sync containers
            SetupContainers(_fields);
        }

        public void DeleteField(StructuredField field)
        {
            _fields.Remove(field);

            // Remove this field from all containers that know about it
            foreach (Container c in _fields
            .Select(f => f.LowestLevelContainer)
            .Distinct()
            .Where(cn => cn.Structures.Contains(field)))
                c.Structures.Remove(field);

            // Sync containers
            SetupContainers(_fields);
        }

        public byte[] EncodeData()
        {
            List<byte> encoded = new List<byte>();

            if (Fields != null)
                foreach (StructuredField field in Fields)
                {
                    encoded.Add(0x5A);
                    encoded.AddRange(field.Introducer);
                    encoded.AddRange(field.Data);
                }

            return encoded.ToArray();
        }

        public class Resource
        {
            // Store .NET's code pages that are 4 digits or less
            private static IReadOnlyList<int> netEncodings = Encoding.GetEncodings().Where(e => e.CodePage <= 9999).Select(e => e.CodePage).ToList();

            public enum eResourceType { Unknown, IMImage, IOCAImage, CodePage, CodedFont, FontCharacterSet, PageSegment }

            public IReadOnlyList<StructuredField> Fields { get; set; }
            public string ResourceName { get; private set; }
            public eResourceType ResourceType { get; private set; }
            public bool IsLoaded => Fields.Any();
            public bool IsEmbedded { get; private set; }
            public bool IsNETCodePage { get; private set; }
            public string Message
            {
                get
                {
                    return
                        IsEmbedded ? "Embedded"
                        : IsLoaded ? "Loaded"
                        : IsNETCodePage ? "File Not Found - Defaulting to .NET's definition"
                        : "File Not Found";
                }
            }

            public Resource(string fName, eResourceType rType, bool embedded = false)
            {
                Fields = new List<StructuredField>();
                ResourceName = fName.ToUpper().Trim();
                ResourceType = rType;
                IsEmbedded = embedded;

                // If we are a code page resource, see if we also exist in .NET
                if (ResourceType == eResourceType.CodePage)
                {
                    // Compare the last four digits of our code page to .NET's existing list.
                    int ourCodePage = 0;
                    if (ResourceName.Length >= 4)
                        int.TryParse(ResourceName.Substring(ResourceName.Length - 4), out ourCodePage);

                    // If we have a match, we will use .NET's, since it's likely a custom file doesn't exist
                    if (ourCodePage > 0) IsNETCodePage = true;
                }
            }
        }
    }
}