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
        public IReadOnlyList<string> ValidationMessages => _validationMessages;

        public AFPFile()
        {
            _fields = new List<StructuredField>();
        }

        public virtual bool LoadData(string path, bool parseData = false)
        {
            try
            {
                // Load all data into main fields property
                _fields = LoadFields(path, parseData);
            }
            catch (Exception ex)
            {
                InvokeErrorEvent($"Error: {ex.Message}");
                return false;
            }

            return true;
        }

        protected List<StructuredField> LoadFields(string path, bool parseData)
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

            // Parse data if needed
            if (parseData)
                foreach (StructuredField field in fields)
                    field.ParseData();

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
                    activeContainers.Add(sf.LowestLevelContainer ?? new Container());

                // Add this field (if new) to each active container's list of fields
                foreach (Container c in activeContainers.Where(c => !c.Structures.Contains(sf)))
                {
                    // Make sure it is added BEFORE any end tags
                    if (c.DirectStructures.Any(s => s.HexID[1] == 0xA9))
                        c.Structures.Insert(c.Structures.Count - 1, sf);
                    else
                        c.Structures.Add(sf);
                }

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

        protected void InvokeErrorEvent(string message)
        {
            ErrorEvent?.Invoke(message);
        }

        #region File/Field Manipulation

        /// <summary>
        /// Adds a new document encapsulation (BDT/EDT tags) to the end of the print file. Will automatically handle nesting if print file tags exist
        /// </summary>
        /// <param name="docName">The optional 8 character name of the document</param>
        /// <returns>The resulting document's container</returns>
        public Container AddDocument(string docName = "")
        {
            Container newContainer = null;
            int indexToInsert = Fields.Count;

            // If BPF/EPF tags exist, make sure the document goes inside them
            if (Fields.Last() is EPF) indexToInsert--;

            // Add the only two required fields for a document - BDT and EDT
            BDT newBDT = new BDT(docName);
            EDT newEDT = new EDT(docName);
            AddFields(new List<StructuredField>() { newBDT, newEDT }, indexToInsert);

            // Set and return the new container
            newContainer = newBDT.LowestLevelContainer;
            return newContainer;
        }

        /// <summary>
        /// Adds a new page container with its required fields to an existing document container
        /// </summary>
        /// <param name="docContainer">The container of the existing document in the AFP to add the page to</param>
        /// <param name="pageName">The optional 8 character name of the new page</param>
        /// <param name="groupName">The optional 8 character name of the new active environment group</param>
        /// <param name="xUnitsPer10Inches">The number of horizontal measurement units on a page/presentation space for every 10 inches</param>
        /// <param name="yUnitsPer10Inches">The number of vertical units on a page/presentation space for every 10 inches</param>
        /// <param name="pageUnitWidth">The number of units that represent the page's width. To convert to inches: (pageUnitWidth / xUnitsPer10Inches) * 10</param>
        /// <param name="pageUnitHeight">The number of units that represent the page's height. To convert to inches: (pageUnitHeight / yUnitsPer10Inches) * 10</param>
        /// <returns>The resulting page's container</returns>
        public Container AddPageToDocument(Container docContainer, string pageName = "", string groupName = "", ushort xUnitsPer10Inches = 3000,
        ushort yUnitsPer10Inches = 3000, ushort pageUnitWidth = 2550, ushort pageUnitHeight = 3300)
        {
            Container pageContainer = null;

            // Verify the container parameter is truly a document
            if (!(docContainer.Structures[0] is BDT && docContainer.Structures.Last() is EDT))
                throw new Exception("The passed container parameter does not appear to be a Document container.");

            // Verify the document exists in the container (and store index to insert if it's ok)
            int indexToInsert = 0;
            for (int i = 0; i < Fields.Count; i++)
            {
                // As soon as we find the begin tag, verify each field in the container matches and break
                if (Fields[i] == docContainer.Structures[0])
                {
                    for (int j = 0; j < docContainer.Structures.Count; j++)
                        if (Fields[i + j] != docContainer.Structures[j])
                            throw new Exception("Invalid container - does not exist in list of fields.");
                    indexToInsert = (i + docContainer.Structures.Count) - 1;
                    break;
                }
            }

            // Create page tags
            BPG newBPG = new BPG(pageName);
            EPG newEPG = new EPG(pageName);

            // A page needs an active environment group
            BAG newBAG = new BAG(groupName);
            EAG newAEG = new EAG(groupName);

            // An active environment group in a page needs both page and presentation text descriptor fields
            PGD newPGD = new PGD(xUnitsPer10Inches, yUnitsPer10Inches, pageUnitWidth, pageUnitHeight);
            PTD1 newPTD = new PTD1(xUnitsPer10Inches, yUnitsPer10Inches, pageUnitWidth, pageUnitHeight);

            // Build the list of new fields and add them to the end of the document
            List<StructuredField> newFields = new List<StructuredField>() { newBPG, newBAG, newPGD, newPTD, newAEG, newEPG };
            AddFields(newFields, indexToInsert);

            // Set and return the created page's container
            pageContainer = newBPG.LowestLevelContainer;
            return pageContainer;
        }

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
            foreach (Container c in Fields.Where(f => f.LowestLevelContainer != null).Select(f => f.LowestLevelContainer).Distinct())
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
    }
}