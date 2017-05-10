using AFPParser.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AFPParser
{
    public class Parser
    {
        public List<Identifier> Identifiers { get; set; }
        public BindingList<StructuredField> AfpFile { get; set; }

        private void LoadIdentifierInfo()
        {
            // Load the list of basic Hex/Abbreviation/Description into memory
            List<string> identifierFileLines = Resources.Field_Identifiers.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            Identifiers = new List<Identifier>();

            foreach (string identifier in identifierFileLines)
            {
                List<string> values = identifier.Split(',').ToList();
                Identifiers.Add(new Identifier(abbreviation: values[0], hexCode: values[1], title: values[2]));
            }

            // Load the list of identifier semantics into memory
            List<string> stringIdentifierInfos = Resources.Identifier_Info.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            int curIdx = 0, nextIdx = 0;
            while (curIdx >= 0)
            {
                curIdx = stringIdentifierInfos.IndexOf(stringIdentifierInfos.Skip(curIdx).FirstOrDefault(i => i[0] == ':'));
                nextIdx = stringIdentifierInfos.IndexOf(stringIdentifierInfos.Skip(curIdx + 1).FirstOrDefault(i => i[0] == ':'));

                // Find identifier with this hex code
                Identifier target = Identifiers.First(i => i.HexCode == stringIdentifierInfos[curIdx].Substring(1, 6));

                // Set description based on constant line number
                target.Semantics.Description = stringIdentifierInfos[curIdx + 1];

                // Get the list of lines that are offsets
                List<string> offsetRows = new List<string>();
                for (int i = curIdx + 2; i < stringIdentifierInfos.Count; i++)
                {
                    if (stringIdentifierInfos[i][0] == ':') break;
                    offsetRows.Add(stringIdentifierInfos[i]);
                }

                target.Semantics.Offsets = LoadOffsets(offsetRows, target.Semantics);

                curIdx = nextIdx;
            }
        }

        private void LoadTriplets()
        {
            Triplets.All = new Dictionary<byte, SemanticsInfo>();

            // Load the list of triplet semantics into memory
            List<string> tripletFileLines = Resources.Triplets.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            int curIdx = 0, nextIdx = 0;
            while (curIdx >= 0)
            {
                curIdx = tripletFileLines.IndexOf(tripletFileLines.Skip(curIdx).FirstOrDefault(i => i[0] == ':'));
                nextIdx = tripletFileLines.IndexOf(tripletFileLines.Skip(curIdx + 1).FirstOrDefault(i => i[0] == ':'));

                // Create new dictionary entry with current triplet info
                SemanticsInfo semantics = new SemanticsInfo();
                byte convertedByte = byte.Parse(tripletFileLines[curIdx].Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                Triplets.All.Add(convertedByte, semantics);

                // Set title and description based on constant line number
                if (curIdx + 1 < tripletFileLines.Count)
                    semantics.Title = tripletFileLines[curIdx + 1];
                if (curIdx + 2 < tripletFileLines.Count)
                    semantics.Description = tripletFileLines[curIdx + 2];
                curIdx += 3;

                // Get the list of lines that are offsets
                List<string> offsetRows = new List<string>();
                for (int i = curIdx; i < tripletFileLines.Count; i++)
                {
                    if (tripletFileLines[i][0] == ':') break;
                    offsetRows.Add(tripletFileLines[i]);
                }

                semantics.Offsets = LoadOffsets(offsetRows, semantics);

                curIdx = nextIdx;
            }
        }

        private List<Offset> LoadOffsets(List<string> rows, SemanticsInfo semantics)
        {
            List<Offset> offsets = new List<Offset>();

            if (rows.Count > 0)
            {
                // First, create a list of split offset string info
                List<string[]> splitOffsetStrings = new List<string[]>();
                splitOffsetStrings.AddRange(rows.Select(r => r.Split(':')));

                // If this is a repeating group of offsets, store the RG info on the identifier
                if (splitOffsetStrings[0][1] == "RSTART")
                {
                    semantics.IsRepeatingGroup = true;
                    semantics.RepeatingGroupStart = int.Parse(splitOffsetStrings[0][0]);
                    splitOffsetStrings.RemoveAt(0);
                }

                // Load any other offsets normally
                foreach (string[] offsetStrings in splitOffsetStrings)
                {
                    Offset oSet = new Offset(int.Parse(offsetStrings[0]), offsetStrings[1], offsetStrings[2]);
                    if (offsetStrings.Length > 3 && !string.IsNullOrWhiteSpace(offsetStrings[3]))
                    {
                        // Get mappings
                        Dictionary<byte, string> mappings = new Dictionary<byte, string>();
                        List<string> rawMappings = offsetStrings[3].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (string m in rawMappings)
                        {
                            List<string> mapping = m.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                            // If there is a range, add each byte in between
                            if (mapping[0][0] == 'R' && mapping[0].Length == 5)
                            {
                                byte begin = byte.Parse(mapping[0].Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                                byte end = byte.Parse(mapping[0].Substring(3, 2), System.Globalization.NumberStyles.HexNumber);

                                while (begin < end)
                                    mappings.Add(begin++, mapping[1]);
                            }
                            // Else just add the single byte map
                            else
                                mappings.Add(byte.Parse(mapping[0], System.Globalization.NumberStyles.HexNumber), mapping[1]);
                        }

                        oSet.Mappings = mappings;
                    }
                    offsets.Add(oSet);
                }
            }

            return offsets;
        }

        public void Parse(string fileName)
        {
            // First, load all identifiers and triplets from text files
            LoadIdentifierInfo();
            LoadTriplets();

            // Then, read all AFP file bytes into memory
            byte[] byteList = File.ReadAllBytes(fileName);

            // Prepare some variables and dynamic funcs...
            int curIdx = 0;
            AfpFile = new BindingList<StructuredField>();
            int curSkippedIdx = 1;
            Func<int, byte[]> takeFromSkippedArray = (int take) =>
            {
                byte[] skippedArray = byteList.Skip(curIdx + curSkippedIdx).Take(take).ToArray();
                curSkippedIdx += take;
                return skippedArray;
            };
            Func<int, byte[]> getProperEndianArray = (int take) =>
            {
                byte[] converted = takeFromSkippedArray(take);
                if (BitConverter.IsLittleEndian) converted = converted.Reverse().ToArray();
                return converted;
            };

            // Next, loop through each 5A block and store a StructuredField object
            while (curIdx < byteList.Length - 1)
            {
                if (byteList[curIdx] != 0x5A)
                {
                    MessageBox.Show($"Unexpected byte at offset 0x{curIdx.ToString("X")}. Is it a true AFP file?", "AFP Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                curSkippedIdx = 1;
                StructuredField field = new StructuredField();

                // Length
                field.Length = BitConverter.ToInt16(getProperEndianArray(2), 0);

                // *** Use the length to determine where the rest of the data resides ***
                string hexIdentifier = BitConverter.ToString(takeFromSkippedArray(3)).Replace("-", "");
                field.Identifier = Identifiers.FirstOrDefault(i => i.HexCode == hexIdentifier);
                field.Flag = takeFromSkippedArray(1)[0];
                field.Sequence = BitConverter.ToInt16(getProperEndianArray(2), 0);
                field.Data = takeFromSkippedArray(field.Length - 8);

                // Append to AFP file
                AfpFile.Add(field);

                // Go to next 5A
                curIdx += field.Length + 1;
            }
        }
    }
}
