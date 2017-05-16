using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AFPParser
{
    [DebuggerDisplay("{StartingIndex}:{DataType}:{Description}")]
    public class Offset
    {
        public int StartingIndex { get; set; }
        public Lookups.DataTypes DataType { get; set; }
        public string Description { get; set; }
        
        public Dictionary<byte, string> Mappings { get; set; }

        public Offset(int startingIdx, Lookups.DataTypes dataType, string description)
        {
            StartingIndex = startingIdx;
            DataType = dataType;
            Description = description;
            Mappings = new Dictionary<byte, string>();
        }

        public static List<Offset> Load(List<string> rows, SemanticsInfo semantics)
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

        public string DisplayDataByType(byte[] data)
        {
            StringBuilder sb = new StringBuilder($"{Description}: ");

            if (Mappings.Any())
                sb.Append(DisplayMappedInfo(data[0]));
            else
                switch (DataType)
                {
                    case "UBIN":
                        // Some UBINs may be 3 bytes. Try to ignore a byte if that happens
                        bool isOdd = data.Length == 3;

                        // AFP is Big Endian
                        if (BitConverter.IsLittleEndian)
                            data = data.Skip(Convert.ToInt32(isOdd)).Reverse().ToArray();
                        else if (isOdd)
                            data = data.Take(data.Length - 1).ToArray();

                        sb.Append(data.Length == 1 ? data[0].ToString()
                            : data.Length == 2 ? BitConverter.ToUInt16(data, 0).ToString()
                            : data.Length == 4 ? BitConverter.ToUInt32(data, 0).ToString()
                            : "(Unknown Numeric Value)");
                        break;

                    case "CHAR":
                    case "CODE":
                        string decoded = Encoding.GetEncoding("IBM037").GetString(data);
                        sb.Append(string.IsNullOrWhiteSpace(decoded) ? "(BLANK)" : decoded);
                        break;

                    default:
                        string hexValue = BitConverter.ToString(data).Replace("-", " ");
                        sb.Append($"({hexValue})");
                        break;
                }

            return sb.ToString();
        }

        private string DisplayMappedInfo(byte data)
        {
            StringBuilder sb = new StringBuilder();

            switch (DataType)
            {
                case "BITS":
                    // Mappings correspond to bit positions
                    Dictionary<byte, string[]> bitsAndDescriptions = new Dictionary<byte, string[]>();
                    foreach (KeyValuePair<byte, string> mapping in Mappings)
                        bitsAndDescriptions.Add(mapping.Key, mapping.Value.Split('|').ToArray());

                    // Display in big Endian if not already
                    BitArray bitInfo = new BitArray(new[] { data });
                    if (BitConverter.IsLittleEndian)
                        bitInfo = new BitArray(bitInfo.Cast<bool>().Reverse().ToArray());

                    // Get the position of the bit, and determine which description to display
                    foreach (KeyValuePair<byte, string[]> kvp in bitsAndDescriptions)
                    {
                        int descIndex = Convert.ToInt32(bitInfo[kvp.Key]); // 0 or 1
                        sb.AppendLine();
                        sb.Append(kvp.Value[descIndex]);
                    }
                    break;

                default:
                    sb.Append(Mappings[data]);
                    break;
            }

            return sb.ToString();
        }
    }
}
