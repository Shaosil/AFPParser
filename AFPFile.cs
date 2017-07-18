using AFPParser.Containers;
using AFPParser.PTXControlSequences;
using AFPParser.StructuredFields;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AFPParser
{
    public class AFPFile
    {
        private List<StructuredField> _fields;
        private List<string> _validationMessages;

        public event Action<string> ErrorEvent;
        public IReadOnlyList<StructuredField> Fields => _fields;
        public List<string> ResourceDirectories { get; set; }
        public IReadOnlyList<Resource> Resources { get; private set; }
        public IReadOnlyList<string> ValidationMessages => _validationMessages;

        public AFPFile()
        {
            _fields = new List<StructuredField>();
            ResourceDirectories = new List<string>() { Environment.CurrentDirectory };
            Resources = new List<Resource>();
        }

        public bool LoadData(string path, bool parseData = false)
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
                ushort length = DataStructure.GetNumericValue<ushort>(lengthBytes);
                ushort sequence = DataStructure.GetNumericValue<ushort>(sequenceBytes);

                // Check the length isn't over what we can read
                if (curIdx + 1 + length > byteList.Length)
                    throw new Exception($"Invalid field length of {length} at offset {curOffsetHex}. Is it a true AFP file?");

                // Get the data
                byte[] data = new byte[length - 8];
                Array.ConstrainedCopy(byteList, curIdx + 9, data, 0, length - 8);

                // Lookup what type of field we need by the structured fields hex dictionary
                Type fieldType = typeof(StructuredFields.UNKNOWN);
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
                // If this is a refresh, and a container exists already, use that instead
                if (sf.HexID[1] == 0xA8)
                    activeContainers.Add(sf.LowestLevelContainer ?? sf.NewContainer);

                // Add this field (if new) to each active container's list of fields
                foreach (Container c in activeContainers.Where(c => !c.Structures.Contains(sf)))
                    c.Structures.Add(sf);

                // Set the field's container list to the currently active ones
                sf.Containers = new List<Container>(activeContainers);

                // If this is an END tag, remove the last matching container from our active container list
                if (sf.HexID[1] == 0xA9)
                {
                    Container matchingBegin = activeContainers.LastOrDefault(c => c.Structures[0].HexID[2] == sf.HexID[2]);
                    if (matchingBegin != null) activeContainers.Remove(matchingBegin);
                }
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

        #region File/Field Manipulation

        /// <summary>
        /// Inserts a field at the specified index in the existing list of Structured Fields
        /// </summary>
        /// <param name="newField"></param> The field that will be inserted
        /// <param name="index"></param> The index at which to insert the new field. Will push existing items down from index position and on.
        public void AddField(StructuredField newField, int index)
        {
            // Insert at specified index
            _fields.Insert(index, newField);

            // Sync containers
            SetupContainers(_fields);
        }

        /// <summary>
        /// Inserts a List of fields at a specified index.
        /// </summary>
        /// <param name="newFields"></param> The group of fields that will be inserted
        /// <param name="index"></param> The index at which to insert the new fields. Will push existing items down from index position and on after all new fields.
        public void AddFields(List<StructuredField> newFields, int index)
        {
            // Insert all, starting at specified index
            for (int i = 0; i < newFields.Count; i++)
                _fields.Insert(index + i, newFields[i]);

            // Sync containers
            SetupContainers(_fields);
        }

        /// <summary>
        /// Removes a field from the list of structured fields
        /// </summary>
        /// <param name="field"></param> The field that will be removed from the list.
        public void DeleteField(StructuredField field)
        {
            _fields.Remove(field);

            // Remove this field from all containers that know about it
            foreach (Container c in _fields
            .Select(f => f.LowestLevelContainer)
            .Distinct()
            .Where(cn => cn != null && cn.Structures.Contains(field)))
                c.Structures.Remove(field);

            // Sync containers
            SetupContainers(_fields);
        }

        /// <summary>
        /// Encodes the existing list of Structured Fields back to a raw byte stream, complete with prefixes, introducers, and data for each line item.
        /// </summary>
        /// <returns>A byte stream containing the raw AFP data, able to be saved to a file</returns>
        public byte[] EncodeData()
        {
            List<byte> encoded = new List<byte>();

            if (Fields != null && Validates())
                foreach (StructuredField field in Fields)
                {
                    encoded.Add(0x5A);
                    encoded.AddRange(field.Introducer);
                    encoded.AddRange(field.Data);
                }

            return encoded.ToArray();
        }

        /// <summary>
        /// Adds a string to the a location on a page, using a specific encoding.
        /// </summary>
        /// <param name="pageContainer"></param> The container of the page which will contain the specified text
        /// <param name="fontCharacterSet"></param> The name of the Font Character Set resource file. Will create a new MCF record if needed.
        /// <param name="inline"></param> The inline (relative horizontal) position offset of the text
        /// <param name="baseline"></param> The baseline (relative vertical) position offset of the text
        /// <param name="text"></param> The string to encode
        /// <param name="encoding"></param> The type of encoding to map characters to. Must correspond to code page mapping parameter.
        /// <param name="interCharSpacing"></param> If specified, overrides the default intercharacter spacing of 0
        /// <param name="codePage"></param> The code page to use when decoding bytes to LIDs
        /// <param name="inlineRotation"></param> The inline (relative horizontal) direction of the text. Must be parallel to baseline rotation.
        /// <param name="baselineRotation"></param> The baseline (relative vertical) direction of the text. Must be parallel to inline rotation.
        public void AddText(Container pageContainer, string fontCharacterSet, short inline, short baseline,
        string text, Encoding encoding, short interCharSpacing = 0, string codePage = "T1DM1252",
        CommonMappings.eRotations inlineRotation = CommonMappings.eRotations.Zero,
        CommonMappings.eRotations baselineRotation = CommonMappings.eRotations.Ninety)
        {
            // Make sure the inline/baseline rotations are parallel
            bool inlineIsHorizontal = inlineRotation == CommonMappings.eRotations.Zero || inlineRotation == CommonMappings.eRotations.OneEighty;
            bool baselineIsHorizontal = baselineRotation == CommonMappings.eRotations.Zero || baselineRotation == CommonMappings.eRotations.OneEighty;
            if (inlineIsHorizontal == baselineIsHorizontal)
                throw new Exception("Error: Inline and baseline rotations must be parallel to each other.");

            // Find the MCF-1 field on this page
            MCF1 mcf = pageContainer.GetStructure<MCF1>();
            IReadOnlyList<MCF1.MCF1Data> mcfData = mcf?.MappedData;
            if (mcfData == null)
                throw new Exception("Error: MCF-1 field could not be found in the specified page.");

            // If the specified font character set does not exist, create it
            byte fontId = mcfData.FirstOrDefault(m => m.FontCharacterSetName == fontCharacterSet)?.ID
                ?? mcf.AddFontDefinition(string.Empty, codePage, fontCharacterSet);

            // Add several sequences to the list based on passed parameters
            List<PTXControlSequence> newSequences = new List<PTXControlSequence>();
            newSequences.Add(new SCFL(fontId, true, true));
            newSequences.Add(new AMI(inline, false, true));
            newSequences.Add(new AMB(baseline, false, true));
            newSequences.Add(new STO(inlineRotation, baselineRotation, false, true));
            if (interCharSpacing != 0) newSequences.Add(new SIA(interCharSpacing, false, true));
            newSequences.Add(new TRN(encoding.GetBytes(text), false, false));

            // Create a new BPT/PTX/EPT at the end of this page
            int indexToInsert = 0;
            for (int i = 0; i < Fields.Count; i++)
                if (Fields[i] == pageContainer.Structures.Last())
                {
                    indexToInsert = i;
                    break;
                }
            AddFields(new List<StructuredField>() { new BPT(), new PTX(newSequences), new EPT() }, indexToInsert);
        }

        /// <summary>
        /// Will ensure all Structured Fields in the file are in a place (container) that they should not be, architecturally.
        /// All error messages are added to the list of validation issues
        /// Object and container heirarchy can be found in the MO:DCA documentation
        /// </summary>
        /// <returns>True if all validation passes</returns>
        public bool Validates()
        {
            _validationMessages = new List<string>();

            // Ensure all containers have open/close tags
            foreach (Container c in Fields.Select(f => f.LowestLevelContainer).Distinct())
                if (c.Structures.Any() && (c.Structures[0].HexID[1] != 0xA8 || c.Structures.Last().HexID[1] != 0xA9))
                {
                    _validationMessages.Add("One or more containers are missing a proper begin and/or end tag.");
                    break;
                }

            // Make sure each begin tag has a container with a matching end tag, and vice versa
            foreach (StructuredField beginOrEnd in Fields.Where(f => f.HexID[1] == 0xA8 || f.HexID[1] == 0xA9))
                if (beginOrEnd.LowestLevelContainer == null || !beginOrEnd.LowestLevelContainer.Structures.Any()    // Container and its structures exist
                || (beginOrEnd.HexID[1] == 0xA8 && beginOrEnd.LowestLevelContainer.Structures[0] != beginOrEnd)     // If begin, first structure is itself
                || (beginOrEnd.HexID[1] == 0xA9 && beginOrEnd.LowestLevelContainer.Structures.Last() != beginOrEnd) // If end, last structure is itself
                || beginOrEnd.LowestLevelContainer.Structures[0].HexID[2] != beginOrEnd.LowestLevelContainer.Structures.Last().HexID[2]) // Begin/end are same type
                {
                    _validationMessages.Add($"One or more begin/end tags are not enveloped in a proper container.");
                    break;
                }

            // Validate each field's positioning in the defined architecture (skip end tags and NOPs
            foreach (StructuredField field in Fields.Where(f => f.GetType() != typeof(StructuredFields.NOP) && f.HexID[1] != 0xA9))
            {
                Type fieldType = field.GetType();
                StructuredField parentField = (StructuredField)(field.HexID[1] == 0xA8 ? field.ParentContainer?.Structures[0] : field.LowestLevelContainer?.Structures[0]);
                Type parentType = parentField?.GetType();

                if (parentType == null)
                {
                    // If this is a print file, or a type that is a child of print file, ignore. BPF/EPF tags are not necessary
                    List<Type> subPFTypes = Lookups.FieldsParentOptions.Where(f => f.Value.Contains(typeof(BPF))).Select(f => f.Key).ToList();

                    if (fieldType != typeof(BPF) && !subPFTypes.Contains(fieldType))
                        _validationMessages.Add($"A {field.Abbreviation} field has no parent container.");
                }
                else
                {
                    // Verify the existing parent container type (if begin tag), or lowest container type is in the list of accepted parent objects
                    if (Lookups.FieldsParentOptions.ContainsKey(fieldType))
                    {
                        if (!Lookups.FieldsParentOptions[fieldType].Contains(parentType))
                            _validationMessages.Add($"A {field.Abbreviation} field has an incorrect parent container of type {parentField.Abbreviation}. " +
                                $"Accepted types are: {string.Join(", ", Lookups.FieldsParentOptions[fieldType].Select(t => t.Name))}.");
                    }

                    // Disable the "missing lookup" validation - we might not be able to cover every scenario
                    //else
                    //    _validationMessages.Add($"Field type {field.Abbreviation} has no heirarchy information in the lookup table.");
                }
            }

            return !_validationMessages.Any();
        }

        #endregion

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